namespace CM4700.Api.Data
{
    public class ScanRequest
    {
        public int Id { get; init; }
        public required Uri Url { get; init; }
        public DateTime DateTimeCreated { get; init; }
        public bool IsCompleted { get; init; }
        public DateTime? DateTimeCompleted { get; init; }
    }
}
