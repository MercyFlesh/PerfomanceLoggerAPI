using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PerfomanceLogger.Infrastructure.Context
{
    public class DbContextFactory : IDesignTimeDbContextFactory<PerfomanceLoggerDbContext>
    {
        public PerfomanceLoggerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PerfomanceLoggerDbContext>();

            string connectionString = args.Length == 0
                ? "Server=(localdb)\\mssqllocaldb;Database=PerfomanceLogger;Trusted_Connection=True;"
                : args[0];

            optionsBuilder.UseSqlServer(connectionString, opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds));
            return new PerfomanceLoggerDbContext(optionsBuilder.Options);
        }
    }
}
