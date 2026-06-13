using CM4700.Api.Data;
using CM4700.Api.Models.Requests;
using CM4700.Api.Models.Responses;
using CM4700.Api.Repository.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CM4700.Api.Controllers
{
    [Route("api/scans")]
    [ApiController]
    public class ScanController : ControllerBase
    {
        private readonly IScanRepository _scanRepository;
        private readonly IBaselineAccessibilityScanner _baselineAccessibilityScanner;
        private readonly IValidator<CreateScanRequest> _createScanRequestValidator;
        private readonly IValidator<CreateScanBatchRequest> _createScanBatchRequestValidator;
        private readonly IValidator<CreateAiFindingRequest> _createAiFindingRequestValidator;
        private readonly IValidator<UpdateScanRequest> _updateScanRequestValidator;

        public ScanController(
            IScanRepository scanRepository,
            IBaselineAccessibilityScanner baselineAccessibilityScanner,
            IValidator<CreateScanRequest> createScanRequestValidator,
            IValidator<CreateScanBatchRequest> createScanBatchRequestValidator,
            IValidator<CreateAiFindingRequest> createAiFindingRequestValidator,
            IValidator<UpdateScanRequest> updateScanRequestValidator)
        {
            _scanRepository = scanRepository;
            _baselineAccessibilityScanner = baselineAccessibilityScanner;
            _createScanRequestValidator = createScanRequestValidator;
            _createScanBatchRequestValidator = createScanBatchRequestValidator;
            _createAiFindingRequestValidator = createAiFindingRequestValidator;
            _updateScanRequestValidator = updateScanRequestValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ScanRequestResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ScanRequestResponse>>> GetAllScanRequestsAsync()
        {
            IEnumerable<ScanRequest> scanRequests = await _scanRepository.GetAllScanRequestsAsync();
            return Ok(scanRequests.Select(MapToResponse));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ScanRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ScanRequestResponse>> GetScanRequestByIdAsync(int id)
        {
            ScanRequest? scanRequest = await _scanRepository.GetScanRequestByIdAsync(id);
            if (scanRequest == null)
            {
                return NotFound();
            }

            return Ok(MapToResponse(scanRequest));
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> CreateScanRequestAsync([FromBody] CreateScanRequest request)
        {
            ValidationResult validationResult = await _createScanRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
            }

            int scanRequestId = await CreateAndRunBaselineScanAsync(request.Url);
            return CreatedAtRoute(new { id = scanRequestId }, scanRequestId);
        }

        [HttpPost("bulk")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<int>>> CreateScanRequestsAsync([FromBody] CreateScanBatchRequest request)
        {
            ValidationResult validationResult = await _createScanBatchRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
            }

            List<int> scanRequestIds = new();
            foreach (string urlText in request.Urls)
            {
                int scanRequestId = await CreateAndRunBaselineScanAsync(urlText);
                scanRequestIds.Add(scanRequestId);
            }

            return StatusCode(StatusCodes.Status201Created, scanRequestIds);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateScanRequestAsync(int id, [FromBody] UpdateScanRequest request)
        {
            ValidationResult validationResult = await _updateScanRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
            }

            Uri url = new(request.Url);
            ScanRequest scanRequest = new()
            {
                Id = id,
                Url = url,
                BaselineScanIsCompleted = request.IsCompleted,
                BaselineScanDateTimeCompleted = request.DateTimeCompleted
            };

            bool updated = await _scanRepository.UpdateScanRequestAsync(id, scanRequest);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("{id}/ai-findings")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> CreateAiFindingAsync(int id, [FromBody] CreateAiFindingRequest request)
        {
            ValidationResult validationResult = await _createAiFindingRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
            }

            ScanRequest? scanRequest = await _scanRepository.GetScanRequestByIdAsync(id);
            if (scanRequest is null)
            {
                return NotFound();
            }

            AiFinding aiFinding = new()
            {
                ScanRequestId = id,
                ModuleName = request.ModuleName,
                ElementType = request.ElementType,
                ElementReference = request.ElementReference,
                ResultLabel = request.ResultLabel,
                Severity = request.Severity,
                Explanation = request.Explanation,
                ConfidenceScore = request.ConfidenceScore
            };

            await _scanRepository.AddAiFindingsAsync([aiFinding]);

            return StatusCode(StatusCodes.Status201Created, id);
        }

        [HttpPost("{id}/ai-complete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MarkAiScanCompletedAsync(int id)
        {
            bool updated = await _scanRepository.MarkAIScanCompletedAsync(id);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteScanRequestAsync(int id)
        {
            bool deleted = await _scanRepository.DeleteScanAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("pending-ai")]
        [ProducesResponseType(typeof(IEnumerable<ScanRequestResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ScanRequestResponse>>> GetPendingAIScanRequestsAsync()
        {
            IEnumerable<ScanRequest> pendingAIScanRequests = (await _scanRepository.GetAllScanRequestsAsync()).Where(x => x.BaselineScanIsCompleted && !x.AIScanIsCompleted);
            return Ok(pendingAIScanRequests.Select(MapToResponse));
        }

        private static ScanRequestResponse MapToResponse(ScanRequest scanRequest)
        {
            return new ScanRequestResponse
            {
                Id = scanRequest.Id,
                Url = scanRequest.Url,
                DateTimeCreated = scanRequest.DateTimeCreated,
                BaselineScanIsCompleted = scanRequest.BaselineScanIsCompleted,
                BaselineScanDateTimeCompleted = scanRequest.BaselineScanDateTimeCompleted,
                AIScanDateTimeCompleted = scanRequest.AIScanDateTimeCompleted,
                AIScanIsCompleted = scanRequest.AIScanIsCompleted
            };
        }

        private async Task<int> CreateAndRunBaselineScanAsync(string urlText)
        {
            ValidationResult validationResult = await _createScanRequestValidator.ValidateAsync(new CreateScanRequest
            {
                Url = urlText
            });

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            Uri url = new(urlText);
            int scanRequestId = await _scanRepository.CreateScanRequestAsync(url);
            IReadOnlyCollection<BaselineFinding> baselineFindings = await _baselineAccessibilityScanner.ScanAsync(scanRequestId, url.ToString());
            await _scanRepository.AddBaselineFindingsAsync(baselineFindings);
            await _scanRepository.MarkBaselineScanCompletedAsync(scanRequestId);

            return scanRequestId;
        }
    }
}
