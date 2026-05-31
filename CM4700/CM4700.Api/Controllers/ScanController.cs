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
        private readonly IValidator<UpdateScanRequest> _updateScanRequestValidator;

        public ScanController(
            IScanRepository scanRepository,
            IBaselineAccessibilityScanner baselineAccessibilityScanner,
            IValidator<CreateScanRequest> createScanRequestValidator,
            IValidator<UpdateScanRequest> updateScanRequestValidator)
        {
            _scanRepository = scanRepository;
            _baselineAccessibilityScanner = baselineAccessibilityScanner;
            _createScanRequestValidator = createScanRequestValidator;
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

            Uri url = new(request.Url);
            int scanRequestId = await _scanRepository.CreateScanRequestAsync(url);
            IReadOnlyCollection<BaselineFinding> baselineFindings = await _baselineAccessibilityScanner.ScanAsync(scanRequestId, url.ToString());
            await _scanRepository.AddBaselineFindingsAsync(baselineFindings);

            return CreatedAtRoute(new { id = scanRequestId }, scanRequestId);
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

        private static ScanRequestResponse MapToResponse(ScanRequest scanRequest)
        {
            return new ScanRequestResponse
            {
                Id = scanRequest.Id,
                Url = scanRequest.Url,
                DateTimeCreated = scanRequest.DateTimeCreated,
                BaselineScanIsCompleted = scanRequest.BaselineScanIsCompleted,
                BaselineScanDateTimeCompleted = scanRequest.BaselineScanDateTimeCompleted,
                AIScanDateTimeCompleted = scanRequest.BaselineScanDateTimeCompleted,
                AIScanIsCompleted = scanRequest.AIScanIsCompleted
            };
        }
    }
}
