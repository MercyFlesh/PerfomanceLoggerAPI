namespace PerfomanceLogger.Domain.Models
{
    public class FilterQuery
    {
        public string? FileName { get; set; }
        public DateTime? StartMinTime { get; set; }
        public DateTime? EndMinTime { get; set; }
        public double? StartMeanMark { get; set; }
        public double? EndMeanMark { get; set; }
        public double? StartMeanTime { get; set; }
        public double? EndMeanTime { get; set; }
    }
}
