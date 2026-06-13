using FluentValidation;

namespace CM4700.Api.Models.Requests
{
    public class CreateScanBatchRequestValidator : AbstractValidator<CreateScanBatchRequest>
    {
        public CreateScanBatchRequestValidator()
        {
            RuleFor(request => request.Urls)
                .NotEmpty();

            RuleForEach(request => request.Urls)
                .NotEmpty()
                .MaximumLength(2048)
                .Must(BeAbsoluteUri)
                .WithMessage("Each URL must be a valid absolute URI.");
        }

        private static bool BeAbsoluteUri(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
