using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PerfomanceLogger.Domain.Models;

namespace PerfomanceLogger.Infrastructure.Context
{
    public class PerfomanceLoggerDbContext : DbContext
    {
        public PerfomanceLoggerDbContext(DbContextOptions<PerfomanceLoggerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Value> Values { get; set; }

        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Value>().HasKey(c => new { c.Id });
            builder.Entity<Result>().HasKey(c => new { c.FileName });

            builder.Entity<Result>().HasMany(c => c.Values).WithOne(e => e.Result);
        }
    }
}
