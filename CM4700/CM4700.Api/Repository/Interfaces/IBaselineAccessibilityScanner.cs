using CM4700.Api.Data;

namespace CM4700.Api.Repository.Interfaces
{
    public interface IBaselineAccessibilityScanner
    {
        Task<List<BaselineFinding>> ScanAsync(int scanRunId, string url);
    }
}
