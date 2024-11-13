using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MSLogistics.Application.Repositories.IStopRepository;
using MSLogistics.Application.Services.StopService;
using MSLogistics.Application.ValueObjects.DTOs.Stop;
using MSLogistics.Domain;
using Xunit;

namespace MSLogistics.UnitTests.ServiceTests.StopServiceTests
{
    public class AddStopsTests
    {
        private readonly StopService _stopService;
        private readonly Mock<IStopRepository> _mockStopRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<StopService>> _mockLogger;

        public AddStopsTests()
        {
            _mockStopRepository = new Mock<IStopRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<StopService>>();

            _stopService = new StopService(_mockStopRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AddStops_ShouldReturnFalse_WhenStopsListIsNullOrEmpty()
        {
            // Act
            var result = await _stopService.AddStops(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddStops_ShouldReturnTrue_WhenStopsAddedSuccessfully()
        {
            // Arrange
            var stopDtos = new List<StopDto>
            {
                new StopDto { Name = "Stop1", CustomerId = Guid.NewGuid() },
                new StopDto { Name = "Stop2", CustomerId = Guid.NewGuid() }
            };
            var stopEntities = stopDtos.Select(dto => new Stop { Name = dto.Name, CustomerId = dto.CustomerId }).ToList();

            _mockMapper.Setup(m => m.Map<IEnumerable<Stop>>(stopDtos)).Returns(stopEntities);
            _mockStopRepository.Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<Stop>>())).ReturnsAsync(true);

            // Act
            var result = await _stopService.AddStops(stopDtos);

            // Assert
            Assert.True(result);
            _mockStopRepository.Verify(repo => repo.AddRangeAsync(It.Is<IEnumerable<Stop>>(stops => stops.Count() == stopEntities.Count)), Times.Once);
        }

        [Fact]
        public async Task AddStops_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var stopDtos = new List<StopDto> { new StopDto { Name = "Stop1", CustomerId = Guid.NewGuid() } };
            var stopEntities = stopDtos.Select(dto => new Stop { Name = dto.Name, CustomerId = dto.CustomerId }).ToList();

            _mockMapper.Setup(m => m.Map<IEnumerable<Stop>>(stopDtos)).Returns(stopEntities);
            _mockStopRepository.Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<Stop>>()))
                               .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _stopService.AddStops(stopDtos);

            // Assert
            Assert.False(result);
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Exception was thrown while inserting a range of records")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
}
