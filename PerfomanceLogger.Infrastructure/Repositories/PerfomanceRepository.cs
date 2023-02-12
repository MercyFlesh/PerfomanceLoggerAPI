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

        public async Task AddOrUpdateDataAsync(List<Value> values, Result result)
        {
            await _context.Results
                .Where(r => r.FileName == result.FileName)
                .ExecuteDeleteAsync();

            await _context.Results.AddAsync(result);
            await _context.Values.AddRangeAsync(values);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Result>> GetResultsAsync(FilterQuery filter)
        {
            var res = await _context.Results.Where(x =>
                (filter.FileName == null || x.FileName.Contains(filter.FileName)) &&
                (filter.StartMinTime == null || x.MinTime >= filter.StartMinTime) &&
                (filter.EndMinTime == null || x.MinTime <= filter.EndMinTime) &&
                (filter.StartMeanMark == null || x.MeanMark >= filter.StartMeanMark) &&
                (filter.EndMeanMark == null || x.MeanMark <= filter.EndMeanMark) &&
                (filter.StartMeanTime == null || x.MeanExecutionTime >= filter.StartMeanTime) &&
                (filter.EndMeanTime == null || x.MeanExecutionTime <= filter.EndMeanTime)
                ).ToListAsync();

            return res;
        }

        public async Task<List<Value>> GetValuesByFileNameAsync(string fileName)
        {
            var values = await _context.Values
                .Where(x => x.FileName == fileName)
                .ToListAsync();

            return values;
        }
    }
}
