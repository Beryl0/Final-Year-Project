using CM4700.Api.Data;

namespace CM4700.Api.Repository.Interfaces
{
    public interface IScanRepository
    {
        Task<int> CreateScanRequestAsync(Uri url);
        Task AddBaselineFindingsAsync(IEnumerable<BaselineFinding> baselineFindings);
        Task<ScanRequest?> GetScanRequestByIdAsync(int id);
        Task<IEnumerable<ScanRequest>> GetAllScanRequestsAsync();
        Task<bool> UpdateScanRequestAsync(int id, ScanRequest scanRequest);
        Task<bool> DeleteScanAsync(int id);
        Task<bool> MarkBaselineScanCompletedAsync(int id);
        Task<bool> MarkAIScanCompletedAsync(int id);
    }
}
