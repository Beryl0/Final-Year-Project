namespace CM4700.Api.Models.Requests
{
    public class CreateScanBatchRequest
    {
        public required List<string> Urls { get; init; }
    }
}
