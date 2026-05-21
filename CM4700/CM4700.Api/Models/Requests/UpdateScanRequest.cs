namespace CM4700.Api.Models.Requests
{
    public class UpdateScanRequest
    {
        public required string Url { get; init; }
        public bool IsCompleted { get; init; }
        public DateTime? DateTimeCompleted { get; init; }
    }
}
