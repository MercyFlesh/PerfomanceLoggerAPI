using Microsoft.EntityFrameworkCore;
using Xunit;
using PerfomanceLogger.Domain.Interfaces;
using PerfomanceLogger.Domain.Models;
using PerfomanceLogger.Infrastructure.Context;
using PerfomanceLogger.Infrastructure.Repositories;

namespace PerfomanceLogger.UnitTests.PerfomanceRepositoryTests
{
    public class PerfomanceRepositoryTest : IDisposable
    {
        private readonly IPerfomanceRepository _repository;
        private readonly PerfomanceLoggerDbContext _perfomanceLoggerContext;

        public PerfomanceRepositoryTest()
        {
            DbContextOptions<PerfomanceLoggerDbContext> options;
            var builder = new DbContextOptionsBuilder<PerfomanceLoggerDbContext>();
            builder = builder.UseInMemoryDatabase(databaseName: "perfomanceLogger");
            options = builder.Options;

            _perfomanceLoggerContext = new PerfomanceLoggerDbContext(options);
            _perfomanceLoggerContext.Database.EnsureDeleted();
            _perfomanceLoggerContext.Database.EnsureCreated();

            _repository = new PerfomanceRepository(_perfomanceLoggerContext);
        }

        public void Dispose()
        {
            _perfomanceLoggerContext.Database.EnsureDeleted();
            _repository.Dispose();
        }

        [Fact]
        public async void AddOrUpdateDataAsync_FirstAdditionFile()
        {
            var expectedResult = new Result { FileName = "test" };
            var expectedValues = new List<Value> {
                    new Value { Date=new DateTime(2022, 3, 18, 9, 18, 17), Time=1744, Mark=1632.472, Result=expectedResult },
                    new Value { Date=new DateTime(2001, 1, 1, 9, 18, 17), Time=560, Mark=680.56, Result=expectedResult },
                    new Value { Date=new DateTime(2000, 1, 1, 9, 18, 17), Time=1744, Mark=1400, Result=expectedResult },
            };

            expectedResult.MinTime = expectedValues[2].Date;
            expectedResult.TotalTime = expectedValues[0].Date - expectedValues[2].Date;
            expectedResult.MeanExecutionTime = (double)expectedValues.Sum(x => x.Time) / expectedValues.Count;
            expectedResult.MeanMark = expectedValues.Sum(x => x.Mark) / expectedValues.Count;
            expectedResult.MedianMark = expectedValues[2].Mark;
            expectedResult.MaxMark = expectedValues[0].Mark;
            expectedResult.MinMark = expectedValues[1].Mark;
            expectedResult.CountRows = expectedValues.Count;

            var exception = await Record.ExceptionAsync(() => _repository.AddOrUpdateDataAsync(expectedValues, expectedResult));
            Assert.Null(exception);

            var actualResult = await _perfomanceLoggerContext.Results.FirstOrDefaultAsync(x => x.FileName == expectedResult.FileName);
            Assert.Equal(expectedResult, actualResult);

            var actualValues = await _perfomanceLoggerContext.Values.Where(v => v.FileName == expectedResult.FileName).ToListAsync();
            Assert.Equal(expectedValues.Count, actualValues.Count);
            for (int i = 0; i < expectedValues.Count; i++)
                Assert.Equal(expectedValues[i], actualValues[i]);
        }

        [Fact]
        public async void TestGetValuesByFileNameAsync()
        {
            var expectedResult = new Result { FileName = "test1" };
            var expectedValues = new List<Value> {
                    new Value { Date=new DateTime(2022, 3, 18, 9, 18, 17), Time=1744, Mark=1632.472, Result=expectedResult },
                    new Value { Date=new DateTime(2001, 1, 1, 9, 18, 17), Time=560, Mark=680.56, Result=expectedResult },
                    new Value { Date=new DateTime(2000, 1, 1, 9, 18, 17), Time=1744, Mark=1400, Result=expectedResult },
            };

            expectedResult.MinTime = expectedValues[2].Date;
            expectedResult.TotalTime = expectedValues[0].Date - expectedValues[2].Date;
            expectedResult.MeanExecutionTime = (double)expectedValues.Sum(x => x.Time) / expectedValues.Count;
            expectedResult.MeanMark = expectedValues.Sum(x => x.Mark) / expectedValues.Count;
            expectedResult.MedianMark = expectedValues[2].Mark;
            expectedResult.MaxMark = expectedValues[0].Mark;
            expectedResult.MinMark = expectedValues[1].Mark;
            expectedResult.CountRows = expectedValues.Count;

            await _perfomanceLoggerContext.Results.AddAsync(expectedResult);
            await _perfomanceLoggerContext.Values.AddRangeAsync(expectedValues);
            await _perfomanceLoggerContext.SaveChangesAsync();

            var actualValues = await _repository.GetValuesByFileNameAsync(expectedResult.FileName);
            Assert.NotNull(actualValues);
            Assert.Equal(expectedValues.Count, actualValues.Count);
            for (int i = 0; i < expectedValues.Count; i++)
                Assert.Equal(expectedValues[i], actualValues[i]);
        }

        [Fact]
        public async void GetResultsAsync_FilteredFilename()
        {
            var expectedResults = new List<Result> {
                new Result { FileName = "testGetResult_1" },
                new Result { FileName = "testGetResult_2" },
                new Result { FileName = "aboba" },
            };

            var expectedValues = new List<List<Value>>();
            for (int i = 0; i < expectedResults.Count; i++)
            {
                expectedValues.Add(new List<Value>());
                await _perfomanceLoggerContext.Results.AddAsync(expectedResults[i]);
                for (int j = 0; j < 3; j++)
                {
                    expectedValues[i].Add(GenerateValue(expectedResults[i]));
                    await _perfomanceLoggerContext.Values.AddAsync(expectedValues[i][j]);
                }
            }
            
            await _perfomanceLoggerContext.SaveChangesAsync();

            var actualResults = await _repository.GetResultsAsync(new FilterQuery { FileName = "testGetResult" });
            Assert.Equal(2, actualResults.Count);
            Assert.Equal(expectedResults[0], actualResults[0]);
            Assert.Equal(expectedResults[1], actualResults[1]);

            actualResults = await _repository.GetResultsAsync(new FilterQuery { FileName = "aboba" });
            Assert.Single(actualResults);
            Assert.Equal(expectedResults[2], actualResults[0]);

            actualResults = await _repository.GetResultsAsync(new FilterQuery { FileName = "brrr" });
            Assert.Empty(actualResults);
        }

        [Fact]
        public async void GetResultsAsync_FilteredFirstStart()
        {
            var expectedResults = new List<Result> {
                new Result { FileName = "filteredFirstStart_1", MinTime=new DateTime(2014, 05, 14) },
                new Result { FileName = "filteredFirstStart_2", MinTime=new DateTime(2021, 06, 17) },
                new Result { FileName = "filteredFirstStart_3", MinTime=new DateTime(2017, 04, 15) },
            };

            var expectedValues = new List<List<Value>>();
            for (int i = 0; i < expectedResults.Count; i++)
            {
                expectedValues.Add(new List<Value>());
                await _perfomanceLoggerContext.Results.AddAsync(expectedResults[i]);
                for (int j = 0; j < 3; j++)
                {
                    expectedValues[i].Add(GenerateValue(expectedResults[i]));
                    await _perfomanceLoggerContext.Values.AddAsync(expectedValues[i][j]);
                }
            }

            await _perfomanceLoggerContext.SaveChangesAsync();

            var actualResults = await _repository.GetResultsAsync(new FilterQuery
            {
                StartMinTime = new DateTime(2014, 05, 10),
                EndMinTime = new DateTime(2018, 06, 11)
            });

            Assert.Equal(2, actualResults.Count);
            Assert.Equal(expectedResults[0], actualResults[0]);
            Assert.Equal(expectedResults[2], actualResults[1]);

            actualResults = await _repository.GetResultsAsync(new FilterQuery
            {
                StartMinTime = new DateTime(2018, 05, 10)
            });

            Assert.Single(actualResults);
            Assert.Equal(expectedResults[1], actualResults[0]);

            actualResults = await _repository.GetResultsAsync(new FilterQuery
            {
                EndMinTime = new DateTime(2016, 06, 11)
            });

            Assert.Single(actualResults);
            Assert.Equal(expectedResults[0], actualResults[0]);
        }

        [Fact]
        public async void GetResultsAsync_FilteredFileNameAndFirstStart()
        {
            var expectedResults = new List<Result> {
                new Result { FileName = "filteredFirstStartFileName_1", MinTime=new DateTime(2014, 05, 14) },
                new Result { FileName = "filteredFirstStartFileName_2", MinTime=new DateTime(2021, 06, 17) },
                new Result { FileName = "abrrrr", MinTime=new DateTime(2017, 04, 15) },
            };

            var expectedValues = new List<List<Value>>();
            for (int i = 0; i < expectedResults.Count; i++)
            {
                expectedValues.Add(new List<Value>());
                await _perfomanceLoggerContext.Results.AddAsync(expectedResults[i]);
                for (int j = 0; j < 3; j++)
                {
                    expectedValues[i].Add(GenerateValue(expectedResults[i]));
                    await _perfomanceLoggerContext.Values.AddAsync(expectedValues[i][j]);
                }
            }

            await _perfomanceLoggerContext.SaveChangesAsync();

            var actualResults = await _repository.GetResultsAsync(new FilterQuery
            {
                FileName = "filteredFirstStartFileName",
                StartMinTime = new DateTime(2015, 05, 10),
                EndMinTime = new DateTime(2022, 06, 11)
            });

            Assert.Single(actualResults);
            Assert.Equal(expectedResults[1], actualResults[0]);
        }

        [Fact]
        public async void GetResultsAsync_FilteredMeanMark()
        {
            var expectedResults = new List<Result> {
                new Result { FileName = "filteredMeanMark_1", MinTime = new DateTime(2014, 05, 14), MeanMark = 1000},
                new Result { FileName = "filteredMeanMark_2", MinTime = new DateTime(2021, 06, 17), MeanMark=700},
                new Result { FileName = "filteredMeanMark", MinTime = new DateTime(2017, 04, 15), MeanMark=400 },
            };

            var expectedValues = new List<List<Value>>();
            for (int i = 0; i < expectedResults.Count; i++)
            {
                expectedValues.Add(new List<Value>());
                await _perfomanceLoggerContext.Results.AddAsync(expectedResults[i]);
                for (int j = 0; j < 3; j++)
                {
                    expectedValues[i].Add(GenerateValue(expectedResults[i]));
                    await _perfomanceLoggerContext.Values.AddAsync(expectedValues[i][j]);
                }
            }

            await _perfomanceLoggerContext.SaveChangesAsync();

            var actualResults = await _repository.GetResultsAsync(new FilterQuery
            {
                StartMeanMark = 500,
                EndMeanMark = 1200
            });

            Assert.Equal(2, actualResults.Count);
            Assert.Equal(expectedResults[0], actualResults[0]);
            Assert.Equal(expectedResults[1], actualResults[1]);
        }

        [Fact]
        public async void GetResultsAsync_FilteredMeanTime()
        {
            var expectedResults = new List<Result> {
                new Result { FileName = "filteredMeanTime_1", MinTime = new DateTime(2014, 05, 14), MeanMark = 1000, MeanExecutionTime = 900},
                new Result { FileName = "filteredMeanTime_2", MinTime = new DateTime(2021, 06, 17), MeanMark = 700, MeanExecutionTime = 700},
                new Result { FileName = "filteredMeanTime", MinTime = new DateTime(2017, 04, 15), MeanMark = 400, MeanExecutionTime = 400},
            };

            var expectedValues = new List<List<Value>>();
            for (int i = 0; i < expectedResults.Count; i++)
            {
                expectedValues.Add(new List<Value>());
                await _perfomanceLoggerContext.Results.AddAsync(expectedResults[i]);
                for (int j = 0; j < 3; j++)
                {
                    expectedValues[i].Add(GenerateValue(expectedResults[i]));
                    await _perfomanceLoggerContext.Values.AddAsync(expectedValues[i][j]);
                }
            }

            await _perfomanceLoggerContext.SaveChangesAsync();

            var actualResults = await _repository.GetResultsAsync(new FilterQuery
            {
                StartMeanTime = 500,
                EndMeanTime = 800
            });

            Assert.Single(actualResults);
            Assert.Equal(expectedResults[1], actualResults[0]);
        }

        [Fact]
        public async void GetResultsAsync_FilteredAll()
        {
            var expectedResults = new List<Result> {
                new Result { FileName = "filteredAll_1", MinTime = new DateTime(2014, 05, 14), MeanMark = 1000, MeanExecutionTime = 900},
                new Result { FileName = "filteredAll_2", MinTime = new DateTime(2021, 06, 17), MeanMark = 700, MeanExecutionTime = 700},
                new Result { FileName = "testcase", MinTime = new DateTime(2017, 04, 15), MeanMark = 400, MeanExecutionTime = 400},
            };

            var expectedValues = new List<List<Value>>();
            for (int i = 0; i < expectedResults.Count; i++)
            {
                expectedValues.Add(new List<Value>());
                await _perfomanceLoggerContext.Results.AddAsync(expectedResults[i]);
                for (int j = 0; j < 3; j++)
                {
                    expectedValues[i].Add(GenerateValue(expectedResults[i]));
                    await _perfomanceLoggerContext.Values.AddAsync(expectedValues[i][j]);
                }
            }

            await _perfomanceLoggerContext.SaveChangesAsync();

            var actualResults = await _repository.GetResultsAsync(new FilterQuery
            {
                FileName = "filteredAll",
                StartMinTime = new DateTime(2012, 05, 14),
                EndMinTime = new DateTime(2022, 05, 14),
                StartMeanTime = 300,
                EndMeanTime = 1000,
                StartMeanMark = 600,
                EndMeanMark = 1000
            });

            Assert.Equal(2, actualResults.Count);
            Assert.Equal(expectedResults[0], actualResults[0]);
            Assert.Equal(expectedResults[1], actualResults[1]);
        }

        private Value GenerateValue(Result res)
        {
            var random = new Random();

            var startDate = new DateTime(2000, 1, 1);
            var endDate = DateTime.Now;
            var year = random.Next(startDate.Year, endDate.Year);
            var month = random.Next(1, 12);
            var day = random.Next(1, DateTime.DaysInMonth(year, month));

            if (year == startDate.Year)
            {
                month = random.Next(startDate.Month, 12);

                if (month == startDate.Month)
                    day = random.Next(startDate.Day, DateTime.DaysInMonth(year, month));
            }

            if (year == endDate.Year)
            {
                month = random.Next(1, endDate.Month);

                if (month == endDate.Month)
                    day = random.Next(1, endDate.Day);
            }

            return new Value
            {
                Result = res,
                Date = new DateTime(year, month, day),
                Time = random.Next(0, 10000),
                Mark = random.Next(0, 5000) / 10000.0
            };
        }
    }
}
