using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfomanceLogger.Domain.Models
{
    public class Result
    {
        public string FileName { get; set; } = null!;
        public TimeSpan TotalTime { get; set; }
        public DateTime MinTime { get; set; }
        public double MeanExecutionTime { get; set; }
        public double MeanMark { get; set; }
        public double MedianMark { get; set; }
        public double MaxMark { get; set; }
        public double MinMark { get; set; }
        public int CountRows { get; set; }
        public List<Value> Values { get; set; } = new List<Value>();
    }
}
