using CM4700.Api.Data;
using CM4700.Api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CM4700.Api.Repository
{
    public class ScanRepository : IScanRepository
    {
        private readonly DatabaseContext _databaseContext;

        public ScanRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<int> CreateScanRequestAsync(Uri url)
        {
            EntityEntry<ScanRequest> scan = _databaseContext.ScanRequests.Add(new ScanRequest
            {
                Url = url,
                DateTimeCreated = DateTime.UtcNow,
                IsCompleted = false
            });
            await _databaseContext.SaveChangesAsync();
            return scan.Entity.Id;
        }

        public async Task<IEnumerable<ScanRequest>> GetAllScanRequestsAsync()
        {
            return await _databaseContext.ScanRequests.ToListAsync();
        }

        public async Task<ScanRequest?> GetScanRequestByIdAsync(int id)
        {
            return await _databaseContext.ScanRequests.FindAsync(id);
        }

        public async Task<bool> UpdateScanRequestAsync(int id, ScanRequest scanRequest)
        {
            ScanRequest? existingScanRequest = await _databaseContext.ScanRequests.FindAsync(id);
            if (existingScanRequest is null)
            {
                return false;
            }

            _databaseContext.Entry(existingScanRequest).CurrentValues.SetValues(new
            {
                existingScanRequest.Id,
                Url = scanRequest.Url,
                existingScanRequest.DateTimeCreated,
                IsCompleted = scanRequest.IsCompleted,
                DateTimeCompleted = scanRequest.DateTimeCompleted
            });
            await _databaseContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteScanAsync(int id)
        {
            ScanRequest? scanRequest = await _databaseContext.ScanRequests.FindAsync(id);
            if (scanRequest is null)
            {
                return false;
            }

            _databaseContext.ScanRequests.Remove(scanRequest);
            await _databaseContext.SaveChangesAsync();

            return true;
        }
    }
}
