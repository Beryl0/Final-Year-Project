using CM4700.Api.Data;
using CM4700.Api.Repository;
using CM4700.Api.Repository.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CM4700.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddUserSecrets<Program>(optional: true);

            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("The connection string 'DefaultConnection' was not found.");

            builder.Services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddScoped<IScanRepository, ScanRepository>();

            builder.Services.AddControllers();

            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            builder.Services.AddOpenApi();

            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
