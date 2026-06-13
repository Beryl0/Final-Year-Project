namespace CM4700.Api.Data
{
    public sealed class AiFinding
    {
        public int Id { get; set; }

        public int ScanRequestId { get; set; }

        public string ModuleName { get; set; } = string.Empty;

        public string ElementType { get; set; } = string.Empty;

        public string ElementReference { get; set; } = string.Empty;

        public string ResultLabel { get; set; } = string.Empty;

        public string Severity { get; set; } = string.Empty;

        public string Explanation { get; set; } = string.Empty;

        public decimal ConfidenceScore { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ScanRequest ScanRequest { get; set; } = null!;
    }
}
