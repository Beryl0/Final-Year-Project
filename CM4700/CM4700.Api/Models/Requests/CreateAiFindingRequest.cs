namespace CM4700.Api.Models.Requests
{
    public class CreateAiFindingRequest
    {
        public required string ModuleName { get; init; }

        public required string ElementType { get; init; }

        public required string ElementReference { get; init; }

        public required string ResultLabel { get; init; }

        public required string Severity { get; init; }

        public required string Explanation { get; init; }

        public decimal ConfidenceScore { get; init; }
    }
}
