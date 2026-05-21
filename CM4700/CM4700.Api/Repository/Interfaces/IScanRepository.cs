using CM4700.Api.Data;

namespace CM4700.Api.Repository.Interfaces
{
    public interface IScanRepository
    {
        Task<int> CreateScanRequestAsync(Uri url);
        Task<ScanRequest?> GetScanRequestByIdAsync(int id);
        Task<IEnumerable<ScanRequest>> GetAllScanRequestsAsync();
        Task<bool> UpdateScanRequestAsync(int id, ScanRequest scanRequest);
        Task<bool> DeleteScanAsync(int id);
    }
}
