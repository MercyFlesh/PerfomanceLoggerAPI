using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace PerfomanceLogger.Infrastructure.Context
{
    public static class DbContextOptionsBuilderExtension
    {
        public static IServiceCollection AddPerfomanceLoggerContext(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PerfomanceLoggerDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("Default");
                options.UseSqlServer(connectionString);
            });

            return services;
        }
    }
}
