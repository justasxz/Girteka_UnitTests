using Girteka_Homework.Controllers;
using Girteka_Homework.Data;
using Girteka_Homework.Data.Models;
using Girteka_Homework.Services;
using Girteka_Homework.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Girteka_UnitTests
{
    public class Tests
    {
        [TestFixture]
        public class AggregationServiceTests
        {
            private IAggregationService _aggregationService;
            private Mock<MySqlDBContext> _dbContextMock;
            private Mock<ILogger<AggregationService>> _loggerMock;
            private string csvFilePath = "csvs/test.csv";

            [SetUp]
            public void Setup()
            {
                _dbContextMock = new Mock<MySqlDBContext>();
                _loggerMock = new Mock<ILogger<AggregationService>>();

                var settings = new Dictionary<string, string> {
            {"PathToCSVDirectory", Path.GetDirectoryName(csvFilePath)},
            };

                IConfiguration configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(settings)
                    .Build();

                _aggregationService = new AggregationService(
                    _dbContextMock.Object,
                    configuration,
                    _loggerMock.Object
                );
            }


            [Test]
            public void ReadCsvs_ReturnsListOfRecords()
            {
                // Arrange
                Directory.CreateDirectory(Path.GetDirectoryName(csvFilePath));

                File.WriteAllText(Path.Combine(AppContext.BaseDirectory, csvFilePath), $"TINKLAS,OBT_PAVADINIMAS,OBJ_GV_TIPAS,OBJ_NUMERIS,P+,PL_T,P-" +
                    $"{Environment.NewLine}Šiaulių regiono tinklas,Namas,G,10572558,0.9874,2022-05-31 00:00:00,0.0" +
                    $"{Environment.NewLine}Alytaus regiono tinklas,Namas,G,11824530,0.5391,2022-05-31 00:00:00,0.0"); // Create a test CSV file



                // Act
                var result = _aggregationService.ReadCsvs();

                // Assert
                Assert.IsInstanceOf<List<Electronic_Data>>(result);
                Assert.IsTrue(result.Count > 0);

                // Clean up
                File.Delete(csvFilePath);
            }
        }
    }
}