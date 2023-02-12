using PerfomanceLogger.Domain.Models;

namespace PerfomanceLogger.Domain.Interfaces
{
    public interface IPerfomanceRepository
    {
        Task AddOrUpdateData(List<Value> values, Result result);
    }
}
