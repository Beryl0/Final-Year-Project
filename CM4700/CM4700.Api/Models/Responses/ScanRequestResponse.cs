namespace CM4700.Api.Models.Responses
{
    public class ScanRequestResponse
    {
        public required int Id { get; init; }
        public required Uri Url { get; init; }
        public required DateTime DateTimeCreated { get; init; }
        public bool BaselineScanIsCompleted { get; set; }
        public DateTime? BaselineScanDateTimeCompleted { get; set; }
        public bool AIScanIsCompleted { get; set; }
        public DateTime? AIScanDateTimeCompleted { get; set; }
    }
}
