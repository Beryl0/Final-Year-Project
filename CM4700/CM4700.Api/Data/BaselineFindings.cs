namespace CM4700.Api.Data
{
    public sealed class BaselineFinding
    {
        public int Id { get; set; }

        public int ScanRequestId { get; set; }

        public string RuleId { get; set; } = string.Empty;

        public string Impact { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Help { get; set; } = string.Empty;

        public string HelpUrl { get; set; } = string.Empty;

        public string ElementHtml { get; set; } = string.Empty;

        public string Target { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ScanRequest ScanRequest { get; set; } = null!;
    }
}
