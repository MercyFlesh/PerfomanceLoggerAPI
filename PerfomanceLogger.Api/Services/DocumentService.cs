using PerfomanceLogger.Domain.Interfaces;
using PerfomanceLogger.Domain.Models;

namespace PerfomanceLogger.Api.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IPerfomanceRepository _perfomanceRepository;

        public DocumentService(IPerfomanceRepository perfomanceRepository)
        {
            _perfomanceRepository = perfomanceRepository;
        }

        public async Task UploadCsv(Stream stream, string fileName)
        {
            var result = new Result{ FileName = fileName };
            var records = new List<Value>();
            using (var streamReader = new StreamReader(stream))
            {
                string? row;
                for (int i = 0; (row = await streamReader.ReadLineAsync()) != null; i++)
                {
                    var record = ValidateValueRecord(row);
                    record.Result = result;
                    records.Add(record);
                }
            }

            UpdateResultValues(records, result);
            await _perfomanceRepository.AddOrUpdateDataAsync(records, result);
        }

        public void UpdateResultValues(List<Value> records, Result result)
        {
            int countRecords = records.Count;
            if (countRecords >= 1 && countRecords <= 10000)
            {
                result.CountRows = countRecords;
                result.MinTime = records.MinBy(x => x.Date)!.Date;
                var maxTime = records.MaxBy(x => x.Date)!.Date;
                result.TotalTime = maxTime - result.MinTime;
                result.MeanExecutionTime = records.Average(x => x.Time);
                result.MeanMark = records.Average(x => x.Mark);

                var recordsOrderedByMark = records.OrderBy(x => x.Mark);
                if (countRecords % 2 == 0)
                    result.MedianMark = recordsOrderedByMark.Skip((countRecords / 2) - 1).Take(2).Average(x => x.Mark);
                else
                    result.MedianMark = recordsOrderedByMark.ElementAt(countRecords / 2).Mark;

                result.MinMark = recordsOrderedByMark.ElementAt(0).Mark;
                result.MaxMark = recordsOrderedByMark.ElementAt(countRecords - 1).Mark;
            }
            else
            {
                throw new ArgumentException($"Incorrect count records in file: count={records.Count}");
            }
        }

        public Value ValidateValueRecord(string csvRow)
        {
            var val = new Value();
            string[] fields = csvRow.Split(';');
            if (fields.Length != 3)
                throw new ArgumentException("Incorrect count collumns in file");

            if (DateTime.TryParseExact(fields[0], "yyyy-MM-dd_HH-mm-ss", provider: null, style: 0, out DateTime date) &&
                date >= new DateTime(2000, 01, 01) && date <= DateTime.Now)
            {
                val.Date = date;
            }
            else
            {
                throw new ArgumentException($"Incorrect date field in record: {csvRow}");
            }

            if (int.TryParse(fields[1], out int time) && time >= 0)
            {
                val.Time = time;
            }
            else
            {
                throw new ArgumentException($"Incorrect time field in record: {csvRow}");
            }

            if (double.TryParse(fields[2], out double mark) && mark >= 0)
            {
                val.Mark = mark;
            }
            else
            {
                throw new ArgumentException($"Incorrect mark field in record: {csvRow}");
            }

            return val;
        }
    }
}
