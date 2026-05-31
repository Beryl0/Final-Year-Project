using Microsoft.EntityFrameworkCore;

namespace CM4700.Api.Data
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        public DbSet<ScanRequest> ScanRequests => Set<ScanRequest>();
        public DbSet<BaselineFinding> BaselineFindings => Set<BaselineFinding>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScanRequest>(entity =>
            {
                entity.HasKey(scanRequest => scanRequest.Id);

                entity.Property(scanRequest => scanRequest.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(scanRequest => scanRequest.Url)
                    .IsRequired()
                    .HasMaxLength(2048)
                    .IsUnicode(false)
                    .HasConversion(
                        uri => uri.ToString(),
                        value => new Uri(value));

                entity.Property(scanRequest => scanRequest.DateTimeCreated)
                    .IsRequired()
                    .HasDefaultValueSql("SYSUTCDATETIME()");

                entity.Property(scanRequest => scanRequest.BaselineScanIsCompleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(scanRequest => scanRequest.BaselineScanDateTimeCompleted);
            });
        }
    }
}
