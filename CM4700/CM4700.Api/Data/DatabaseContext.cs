using Microsoft.EntityFrameworkCore;

namespace CM4700.Api.Data
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        public DbSet<ScanRequest> ScanRequests => Set<ScanRequest>();
        public DbSet<BaselineFinding> BaselineFindings => Set<BaselineFinding>();
        public DbSet<AiFinding> AiFindings => Set<AiFinding>();

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

            modelBuilder.Entity<AiFinding>(entity =>
            {
                entity.HasKey(aiFinding => aiFinding.Id);

                entity.Property(aiFinding => aiFinding.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(aiFinding => aiFinding.ModuleName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(aiFinding => aiFinding.ElementType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(aiFinding => aiFinding.ElementReference)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(aiFinding => aiFinding.ResultLabel)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(aiFinding => aiFinding.Severity)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(aiFinding => aiFinding.Explanation)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(aiFinding => aiFinding.ConfidenceScore)
                    .HasPrecision(5, 4);

                entity.Property(aiFinding => aiFinding.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("SYSUTCDATETIME()");

                entity.HasOne(aiFinding => aiFinding.ScanRequest)
                    .WithMany()
                    .HasForeignKey(aiFinding => aiFinding.ScanRequestId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
