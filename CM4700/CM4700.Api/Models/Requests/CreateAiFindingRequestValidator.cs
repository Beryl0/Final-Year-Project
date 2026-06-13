using FluentValidation;

namespace CM4700.Api.Models.Requests
{
    public class CreateAiFindingRequestValidator : AbstractValidator<CreateAiFindingRequest>
    {
        public CreateAiFindingRequestValidator()
        {
            RuleFor(request => request.ModuleName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(request => request.ElementType)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(request => request.ElementReference)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(request => request.ResultLabel)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(request => request.Severity)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(request => request.Explanation)
                .NotEmpty()
                .MaximumLength(4000);

            RuleFor(request => request.ConfidenceScore)
                .InclusiveBetween(0, 1);
        }
    }
}
