using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfomanceLogger.Domain.Models
{
    public class Value
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Time { get; set; }
        public double Mark { get; set; }
        public Result? Result { get; set; }
    }
}
