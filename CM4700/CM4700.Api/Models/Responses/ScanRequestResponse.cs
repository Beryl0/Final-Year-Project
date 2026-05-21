namespace CM4700.Api.Models.Responses
{
    public class ScanRequestResponse
    {
        public required int Id { get; init; }
        public required Uri Url { get; init; }
        public required DateTime DateTimeCreated { get; init; }
        public required bool IsCompleted { get; init; }
        public DateTime? DateTimeCompleted { get; init; }
    }
}
