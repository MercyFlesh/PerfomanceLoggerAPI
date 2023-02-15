﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PerfomanceLogger.Infrastructure.Context;

#nullable disable

namespace PerfomanceLogger.Infrastructure.Migrations
{
    [DbContext(typeof(PerfomanceLoggerDbContext))]
    partial class PerfomanceLoggerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PerfomanceLogger.Domain.Models.Result", b =>
                {
                    b.Property<string>("FileName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("CountRows")
                        .HasColumnType("int");

                    b.Property<double>("MaxMark")
                        .HasColumnType("float");

                    b.Property<double>("MeanExecutionTime")
                        .HasColumnType("float");

                    b.Property<double>("MeanMark")
                        .HasColumnType("float");

                    b.Property<double>("MedianMark")
                        .HasColumnType("float");

                    b.Property<double>("MinMark")
                        .HasColumnType("float");

                    b.Property<DateTime>("MinTime")
                        .HasColumnType("datetime2");

                    b.Property<long>("TotalTime")
                        .HasColumnType("bigint");

                    b.HasKey("FileName");

                    b.ToTable("Results");
                });

            modelBuilder.Entity("PerfomanceLogger.Domain.Models.Value", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("FileName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Mark")
                        .HasColumnType("float");

                    b.Property<int>("Time")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FileName");

                    b.ToTable("Values");
                });

            modelBuilder.Entity("PerfomanceLogger.Domain.Models.Value", b =>
                {
                    b.HasOne("PerfomanceLogger.Domain.Models.Result", "Result")
                        .WithMany("Values")
                        .HasForeignKey("FileName")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Result");
                });

            modelBuilder.Entity("PerfomanceLogger.Domain.Models.Result", b =>
                {
                    b.Navigation("Values");
                });
#pragma warning restore 612, 618
        }
    }
}
