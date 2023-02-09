using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfomanceLogger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    FileName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MinTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MeanExecutionTime = table.Column<double>(type: "float", nullable: false),
                    MeanMark = table.Column<double>(type: "float", nullable: false),
                    MeadianMark = table.Column<double>(type: "float", nullable: false),
                    MaxMark = table.Column<double>(type: "float", nullable: false),
                    MinMark = table.Column<double>(type: "float", nullable: false),
                    CountRows = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.FileName);
                });

            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time = table.Column<long>(type: "bigint", nullable: false),
                    Mark = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "Values");
        }
    }
}
