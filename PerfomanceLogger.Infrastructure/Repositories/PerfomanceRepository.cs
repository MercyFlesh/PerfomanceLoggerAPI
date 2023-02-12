using Microsoft.EntityFrameworkCore;
using PerfomanceLogger.Infrastructure.Context;
using PerfomanceLogger.Domain.Interfaces;
using PerfomanceLogger.Domain.Models;

namespace PerfomanceLogger.Infrastructure.Repositories
{
    public class PerfomanceRepository : IPerfomanceRepository
    {
        private readonly PerfomanceLoggerDbContext _context;

        public PerfomanceRepository()
        {
            _context = new DbContextFactory().CreateDbContext(new string[0]);
        }

        public PerfomanceRepository(PerfomanceLoggerDbContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task AddOrUpdateData(List<Value> values, Result result)
        {
            await _context.Results
                .Where(r => r.FileName == result.FileName)
                .ExecuteDeleteAsync();

            await _context.Results.AddAsync(result);
            await _context.Values.AddRangeAsync(values);

            await _context.SaveChangesAsync();
        }
    }
}
