using System.Text;
using Xunit;
using Moq;
using PerfomanceLogger.Api.Services;
using PerfomanceLogger.Domain.Interfaces;
using PerfomanceLogger.Domain.Models;

namespace PerfomanceLogger.UnitTests.DocumentServiceTests
{
    public class DocumentServiceTest
    {
        private DocumentService documentService;

        private Mock<IPerfomanceRepository> mockPerfomanceRepository;

        public DocumentServiceTest()
        {
            mockPerfomanceRepository = new Mock<IPerfomanceRepository>();
            mockPerfomanceRepository.Setup(rep => rep.AddOrUpdateDataAsync(It.IsAny<List<Value>>(), It.IsAny<Result>()));
            documentService = new DocumentService(mockPerfomanceRepository.Object);
        }

        [Fact]
        public void ValidateValueRecord_ParseCorrectCsvRecord()
        {
            var corectRecordsCases = new Dictionary<string, Value> {
                {
                    "2022-03-18_09-18-17;1744;1632,472",
                    new Value { Date=new DateTime(2022, 3, 18, 9, 18, 17), Time=1744, Mark=1632.472 }
                },
                {
                    "2000-01-01_09-18-17;1744;1632",
                    new Value { Date=new DateTime(2000, 1, 1, 9, 18, 17), Time=1744, Mark=1632 }
                },
                {
                    "2001-01-01_09-18-17;0;0",
                    new Value { Date=new DateTime(2001, 1, 1, 9, 18, 17), Time=0, Mark=0 }
                }
            };

            foreach (var (csvRecord, expectedValue) in corectRecordsCases)
            {
                var exception = Record.Exception(() => documentService.ValidateValueRecord(csvRecord));
                Assert.Null(exception);

                Value actualValue = documentService.ValidateValueRecord(csvRecord);
                Assert.Equal(expectedValue.Date, actualValue.Date);
                Assert.Equal(expectedValue.Time, actualValue.Time);
                Assert.Equal(expectedValue.Mark, actualValue.Mark);
            }
        }

        [Theory]
        [InlineData("2022-03-18_09-18-17;1744;")]
        [InlineData(";1744;1632")]
        [InlineData(";;")]
        [InlineData("")]
        public void ValidateValueRecord_ParseRecordWithEmptyFields_ThrowsArgumentException(string incorrectRecord)
        {
            Action action = () => documentService.ValidateValueRecord(incorrectRecord);
            Assert.Throws<ArgumentException>(action);
        }

        [Theory]
        [InlineData("2022-03-18_09-18-17,1744,1632.472")]
        [InlineData("2022-03-18_09-18-17;1744;1632,472;1546")]
        public void ValidateValueRecord_ParseRecordWithIncorerctFormat_ThrowsArgumentException(string incorrectRecord)
        {
            Action action = () => documentService.ValidateValueRecord(incorrectRecord);
            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Incorrect count collumns in file", exception.Message);
            
        }

        [Fact]
        public void ValidateValueRecord_IncorrectDateFormat_ThrowsArgumentException()
        {
            string incorrectRecord = "18/03/2022 09:18:17;1744;1632,472";
            Action action = () => documentService.ValidateValueRecord(incorrectRecord);
            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal($"Incorrect date field in record: {incorrectRecord}", exception.Message);
        }

        [Fact]
        public void ValidateValueRecord_InvalidDate_ThrowsArgumentException()
        {
            string[] incorrectRecords = {
                "1999-31-12_09-18-17;1744;1632,472",
                $"{DateTime.Now.AddDays(5).ToString("yyyy-MM-dd_HH-mm-ss")};1744;1632,472"
            };

            foreach(var record in incorrectRecords)
            {
                Action action = () => documentService.ValidateValueRecord(record);
                var exception = Assert.Throws<ArgumentException>(action);
                Assert.Equal($"Incorrect date field in record: {record}", exception.Message);
            }
        }

        [Fact]
        public void ValidateValueRecord_InvalidTime_ThrowsArgumentException()
        {
            string incorrectRecord = "2022-03-18_09-18-17;-1;1632,472";
            Action action = () => documentService.ValidateValueRecord(incorrectRecord);
            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal($"Incorrect time field in record: {incorrectRecord}", exception.Message);
        }

        [Fact]
        public void ValidateValueRecord_InvalidMark_ThrowsArgumentException()
        {
            string incorrectRecord = "2022-03-18_09-18-17;1744;-1";
            Action action = () => documentService.ValidateValueRecord(incorrectRecord);
            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal($"Incorrect mark field in record: {incorrectRecord}", exception.Message);
        }

        [Fact]
        public void UpdateResultValues_InvalidCountRecords_ThrowsArgumentException()
        {
            var emptyListRecords = new List<Value>();
            var result = new Result();

            Action action = () => documentService.UpdateResultValues(emptyListRecords, result);
            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal($"Incorrect count records in file: count={emptyListRecords.Count}", exception.Message);
            
            var overflowRecords = Enumerable.Range(0, 10001).Aggregate(new List<Value>(), (list, x) => 
            { 
                list.Add(new Value()); 
                return list;
            });

            action = () => documentService.UpdateResultValues(overflowRecords, result);
            exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal($"Incorrect count records in file: count={overflowRecords.Count}", exception.Message);
        }

        [Fact]
        public void UpdateResultValues_ValidValues()
        {
            var actualResult = new Result();
            var values = new List<Value> {
                    new Value { Date=new DateTime(2022, 3, 18, 9, 18, 17), Time=1744, Mark=1632.472 },
                    new Value { Date=new DateTime(2001, 1, 1, 9, 18, 17), Time=560, Mark=680.56 },
                    new Value { Date=new DateTime(2000, 1, 1, 9, 18, 17), Time=1744, Mark=1400 },
            };

            documentService.UpdateResultValues(values, actualResult);

            var expectedMinTime = values[2].Date;
            var expectedTotalTime = values[0].Date - values[2].Date;
            var expectedMeanExecutionTime = (double)values.Sum(x => x.Time) / values.Count;
            var expectedMeanMark = values.Sum(x => x.Mark) / values.Count;
            var expectedMedianMark = values[2].Mark;
            var expectedMaxMark = values[0].Mark;
            var expectedMinMark = values[1].Mark;
            var expectedCountRows = values.Count;

            Assert.Equal(expectedMinTime, actualResult.MinTime);
            Assert.Equal(expectedTotalTime, actualResult.TotalTime);
            Assert.Equal(expectedMeanExecutionTime, actualResult.MeanExecutionTime);
            Assert.Equal(expectedMeanMark, actualResult.MeanMark);
            Assert.Equal(expectedMedianMark, actualResult.MedianMark);
            Assert.Equal(expectedMinMark, actualResult.MinMark);
            Assert.Equal(expectedMaxMark, actualResult.MaxMark);
            Assert.Equal(expectedCountRows, actualResult.CountRows);
        }

        [Fact]
        public void UpdateResultValues_CheckMeadianMarkWithEvenCountRecords()
        {
            var actualResult = new Result();
            var values = new List<Value> {
                    new Value { Date=new DateTime(2022, 3, 18, 9, 18, 17), Time=1744, Mark=1632.472 },
                    new Value { Date=new DateTime(2001, 1, 1, 9, 18, 17), Time=560, Mark=680.56 },
                    new Value { Date=new DateTime(2000, 1, 1, 9, 18, 17), Time=1744, Mark=1400 },
                    new Value { Date=new DateTime(2007, 1, 1, 9, 18, 17), Time=1744, Mark=1500 },
            };

            var expectedMedianMark = (values[2].Mark + values[3].Mark) / 2;
            documentService.UpdateResultValues(values, actualResult);

            Assert.Equal(expectedMedianMark, actualResult.MedianMark);
        }

        [Fact]
        public async void TestUploadCsv()
        {
            string testCsv = "2022-03-18_09-18-17;1744;1632,472\r\n" +
                "2000-01-01_09-18-17;1744;1632\r\n" +
                "2001-01-01_09-18-17;560;1400";

            var exception = await Record.ExceptionAsync(() => documentService.UploadCsv(new MemoryStream(Encoding.UTF8.GetBytes(testCsv)), "test"));
            Assert.Null(exception);
        }
    }
}
