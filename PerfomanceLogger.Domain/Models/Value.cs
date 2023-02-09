using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfomanceLogger.Domain.Models
{
    public class Value
    {
        public int Id { get; set; }

        public string FileName { get; init; } = null!;

        public DateTime Date { get; init; }

        public uint Time { get; set; }

        public double Mark { get; set; }
    }
}
