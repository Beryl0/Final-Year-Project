namespace CM4700.Api.Data
{
    public class ScanRequest
    {
        public int Id { get; init; }
        public required Uri Url { get; init; }
        public DateTime DateTimeCreated { get; init; }
        public bool BaselineScanIsCompleted { get; set; }
        public DateTime? BaselineScanDateTimeCompleted { get; set; }
        public bool AIScanIsCompleted { get; set; }
        public DateTime? AIScanDateTimeCompleted { get; set; }
    }
}
