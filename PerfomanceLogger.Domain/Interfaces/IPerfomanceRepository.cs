using PerfomanceLogger.Domain.Models;

namespace PerfomanceLogger.Domain.Interfaces
{
    public interface IPerfomanceRepository : IDisposable
    {
        Task AddOrUpdateDataAsync(List<Value> values, Result result);
        Task<List<Result>> GetResultsAsync(FilterQuery filter);
        Task<List<Value>> GetValuesByFileNameAsync(string fileName);
    }
}
