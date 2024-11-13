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
    public class UpdateStopsTests
    {
        private readonly StopService _stopService;
        private readonly Mock<IStopRepository> _mockStopRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<StopService>> _mockLogger;

        public UpdateStopsTests()
        {
            _mockStopRepository = new Mock<IStopRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<StopService>>();

            _stopService = new StopService(_mockStopRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task UpdateStops_ShouldReturnFalse_WhenStopsListIsNullOrEmpty()
        {
            // Act
            var result = await _stopService.UpdateStops(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateStops_ShouldReturnTrue_WhenStopsUpdatedSuccessfully()
        {
            // Arrange
            var stopDtos = new List<StopDto>
            {
                new StopDto { Id = Guid.NewGuid(), Name = "Stop1", CustomerId = Guid.NewGuid() },
                new StopDto { Id = Guid.NewGuid(), Name = "Stop2", CustomerId = Guid.NewGuid() }
            };

            var existingStops = stopDtos.Select(dto => new Stop { Id = dto.Id, Name = dto.Name, CustomerId = dto.CustomerId }).ToList();

            _mockStopRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                               .ReturnsAsync((Guid id) => existingStops.FirstOrDefault(s => s.Id == id));
            _mockMapper.Setup(m => m.Map(It.IsAny<StopDto>(), It.IsAny<Stop>()));
            _mockStopRepository.Setup(repo => repo.UpdateRangeAsync(It.IsAny<IEnumerable<Stop>>())).ReturnsAsync(true);

            // Act
            var result = await _stopService.UpdateStops(stopDtos);

            // Assert
            Assert.True(result);
            _mockStopRepository.Verify(repo => repo.UpdateRangeAsync(It.Is<IEnumerable<Stop>>(stops => stops.Count() == stopDtos.Count)), Times.Once);
        }

        [Fact]
        public async Task UpdateStops_ShouldLogError_WhenStopNotFoundForUpdate()
        {
            // Arrange
            var stopDtos = new List<StopDto>
            {
                new StopDto { Id = Guid.NewGuid(), Name = "NonExistentStop", CustomerId = Guid.NewGuid() }
            };

            _mockStopRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Stop)null);

            // Act
            var result = await _stopService.UpdateStops(stopDtos);

            // Assert
            Assert.False(result);
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Stop with ID {stopDtos.First().Id} not found for updating.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateStops_ShouldLogError_WhenNoValidStopsToUpdate()
        {
            // Arrange
            var stopDtos = new List<StopDto>
            {
                new StopDto { Id = Guid.NewGuid(), Name = "NonExistentStop", CustomerId = Guid.NewGuid() }
            };

            _mockStopRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Stop)null);

            // Act
            var result = await _stopService.UpdateStops(stopDtos);

            // Assert
            Assert.False(result);
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No valid stops found to update.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateStops_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var stopDtos = new List<StopDto>
            {
                new StopDto { Id = Guid.NewGuid(), Name = "Stop1", CustomerId = Guid.NewGuid() }
            };
            var existingStop = new Stop { Id = stopDtos.First().Id, Name = "Stop1", CustomerId = stopDtos.First().CustomerId };

            _mockStopRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(existingStop);
            _mockStopRepository.Setup(repo => repo.UpdateRangeAsync(It.IsAny<IEnumerable<Stop>>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _stopService.UpdateStops(stopDtos);

            // Assert
            Assert.False(result);
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Exception was thrown while updating a range of records")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
}

