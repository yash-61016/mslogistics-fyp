using System;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MSLogistics.Application.Repositories.IStopRepository;
using MSLogistics.Application.Services.StopService;
using MSLogistics.Domain;
using Xunit;

namespace MSLogistics.UnitTests.ServiceTests.StopServiceTests
{
    public class DeleteStopsTests
    {
        private readonly StopService _stopService;
        private readonly Mock<IStopRepository> _mockStopRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<StopService>> _mockLogger;

        public DeleteStopsTests()
        {
            _mockStopRepository = new Mock<IStopRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<StopService>>();

            _stopService = new StopService(_mockStopRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task DeleteStops_ShouldReturnFalse_WhenIdsIsNullOrEmpty()
        {
            // Act
            var result = await _stopService.DeleteStops(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteStops_ShouldLogError_WhenStopNotFound()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };
            _mockStopRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Stop)null);

            // Act
            await _stopService.DeleteStops(ids);

            // Assert
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Stop with ID")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteStops_ShouldReturnTrue_WhenStopsDeletedSuccessfully()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var stops = ids.Select(id => new Stop { Id = id }).ToList();

            _mockStopRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                               .ReturnsAsync((Guid id) => stops.FirstOrDefault(s => s.Id == id));
            _mockStopRepository.Setup(repo => repo.DeleteRangeAsync(It.IsAny<IEnumerable<Guid>>()))
                               .ReturnsAsync(true);

            // Act
            var result = await _stopService.DeleteStops(ids);

            // Assert
            Assert.True(result);
            _mockStopRepository.Verify(repo => repo.DeleteRangeAsync(It.Is<IEnumerable<Guid>>(ids => ids.Count() == stops.Count)), Times.Once);
        }

        [Fact]
        public async Task DeleteStops_ShouldReturnFalse_WhenExceptionIsThrown()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };
            _mockStopRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                               .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _stopService.DeleteStops(ids);

            // Assert
            Assert.False(result);
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Exception was thrown while deleting a range")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
}

