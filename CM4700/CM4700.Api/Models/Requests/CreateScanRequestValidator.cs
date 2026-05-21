using FluentValidation;

namespace CM4700.Api.Models.Requests
{
    public class CreateScanRequestValidator : AbstractValidator<CreateScanRequest>
    {
        public CreateScanRequestValidator()
        {
            RuleFor(request => request.Url)
                .NotEmpty()
                .MaximumLength(2048)
                .Must(BeAbsoluteUri)
                .WithMessage("The Url field must be a valid absolute URI.");
        }

        private static bool BeAbsoluteUri(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
