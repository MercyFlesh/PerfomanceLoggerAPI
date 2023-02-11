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

        public string FileName { get; set; } = null!;

        public DateTime Date { get; set; }

        public int Time { get; set; }

        public double Mark { get; set; }
    }
}
