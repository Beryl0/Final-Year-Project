using FluentValidation;

namespace CM4700.Api.Models.Requests
{
    public class UpdateScanRequestValidator : AbstractValidator<UpdateScanRequest>
    {
        public UpdateScanRequestValidator()
        {
            RuleFor(request => request.Url)
                .NotEmpty()
                .MaximumLength(2048)
                .Must(BeAbsoluteUri)
                .WithMessage("The Url field must be a valid absolute URI.");

            RuleFor(request => request.DateTimeCompleted)
                .Null()
                .When(request => !request.IsCompleted)
                .WithMessage("DateTimeCompleted must be null when IsCompleted is false.");

            RuleFor(request => request.DateTimeCompleted)
                .NotNull()
                .When(request => request.IsCompleted)
                .WithMessage("DateTimeCompleted is required when IsCompleted is true.");
        }

        private static bool BeAbsoluteUri(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
