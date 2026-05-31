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
                BaselineScanIsCompleted = false
            });
            await _databaseContext.SaveChangesAsync();
            return scan.Entity.Id;
        }

        public async Task AddBaselineFindingsAsync(IEnumerable<BaselineFinding> baselineFindings)
        {
            List<BaselineFinding> findings = baselineFindings.ToList();
            if (findings.Count == 0)
            {
                return;
            }

            await _databaseContext.BaselineFindings.AddRangeAsync(findings);
            await _databaseContext.SaveChangesAsync();
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
                scanRequest.Url,
                existingScanRequest.DateTimeCreated,
                scanRequest.BaselineScanIsCompleted,
                scanRequest.BaselineScanDateTimeCompleted,
                scanRequest.AIScanIsCompleted,
                scanRequest.AIScanDateTimeCompleted
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

        public async Task<bool> MarkBaselineScanCompletedAsync(int id)
        {
            ScanRequest? existingScanRequest = await _databaseContext.ScanRequests.FindAsync(id);

            if (existingScanRequest is null)
            {
                return false;
            }

            existingScanRequest.BaselineScanIsCompleted = true;
            existingScanRequest.BaselineScanDateTimeCompleted = DateTime.UtcNow;

            await _databaseContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkAIScanCompletedAsync(int id)
        {
            ScanRequest? existingScanRequest = await _databaseContext.ScanRequests.FindAsync(id);

            if (existingScanRequest is null)
            {
                return false;
            }

            existingScanRequest.AIScanIsCompleted = true;
            existingScanRequest.AIScanDateTimeCompleted = DateTime.UtcNow;

            await _databaseContext.SaveChangesAsync();

            return true;
        }
    }
}
