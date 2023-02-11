using System.IO;
using PerfomanceLogger.Domain.Interfaces;
using PerfomanceLogger.Domain.Models;

namespace PerfomanceLogger.Api.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ILogger _logger;

        public DocumentService(ILogger<DocumentService> logger)
        {
            _logger = logger;
        }

        public async void UploadCsv(Stream stream, string fileName)
        {
            var records = new List<Value>();
            using (var streamReader = new StreamReader(stream))
            {
                string? row;
                for (int i = 0; (row = await streamReader.ReadLineAsync()) != null; i++)
                {
                    var valData = ValidateData(row);
                    valData.FileName = fileName;
                    valData.Id = i;
                    records.Add(valData);
                }
            }

            int countRecords = records.Count;
            if (countRecords > 1 && countRecords < 10000)
            {
                var minTime = records.MinBy(x => x.Date)!.Date;
                var maxTime = records.MaxBy(x => x.Date)!.Date;
                TimeSpan totalTime = maxTime - minTime;
                double meanTime = records.Average(x => x.Time);
                double meanMark = records.Average(x => x.Mark);
                
                var recordsOrderedByMark = records.OrderBy(x => x);
                double medianMark;
                if (countRecords % 2 == 0)
                    medianMark = recordsOrderedByMark.Skip((countRecords / 2) - 1).Take(2).Average(x => x.Mark);
                else
                    medianMark = recordsOrderedByMark.ElementAt(countRecords / 2).Mark;

                var minMark = recordsOrderedByMark.ElementAt(0).Mark;
                var maxMark = recordsOrderedByMark.ElementAt(countRecords - 1).Mark;

                var result = new Result
                {
                    FileName = fileName,
                    TotalTime = totalTime,
                    MinTime = minTime,
                    MeanExecutionTime = meanTime,
                    MeanMark = meanMark,
                    MeadianMark = medianMark,
                    MinMark = minMark,
                    MaxMark = maxMark,
                    CountRows = countRecords
                };

            }
            else
            {
                throw new ArgumentException($"Incorrect count records in file: count={records.Count}");
            }
        }

        public Value ValidateData(string csvRow)
        {
            var val = new Value();
            string[] fields = csvRow.Split(';');
            if (fields.Length != 3)
                throw new ArgumentException("Incorrect count collumns in file");

            if (DateTime.TryParse(fields[0], out DateTime date) &&
                date >= new DateTime(2000, 01, 01) && date <= DateTime.Now)
            {
                val.Date = date;
            }
            else
            {
                throw new ArgumentException($"Incorrect date field in record {csvRow}");
            }

            if (int.TryParse(fields[1], out int time) && time >= 0)
            {
                val.Time = time;
            }
            else
            {
                throw new ArgumentException($"Incorrect time field in record {csvRow}");
            }

            if (double.TryParse(fields[2], out double mark) && mark >= 0)
            {
                val.Mark = mark;
            }
            else
            {
                throw new ArgumentException($"Incorrect mark field in record {csvRow}");
            }

            return val;
        }
    }
}
