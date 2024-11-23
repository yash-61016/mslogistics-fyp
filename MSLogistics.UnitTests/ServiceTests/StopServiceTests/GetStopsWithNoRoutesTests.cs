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
	public class GetStopsWithNoRoutesTests
	{
        private readonly StopService _stopService;
        private readonly Mock<IStopRepository> _mockStopRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<StopService>> _mockLogger;

        public GetStopsWithNoRoutesTests()
        {
            _mockStopRepository = new Mock<IStopRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<StopService>>();

            _stopService = new StopService(_mockStopRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetStopsWithNoRoutes_ShouldReturnStopsWithNoRoutes()
        {
            // Arrange
            var stops = new List<Stop>
            {
                new Stop { Id = Guid.NewGuid(), Name = "Stop 1", RouteId = null },
                new Stop { Id = Guid.NewGuid(), Name = "Stop 2", RouteId = null },
                new Stop { Id = Guid.NewGuid(), Name = "Stop 3", RouteId = Guid.NewGuid() } // This one should not be included
            };

            var stopDtos = new List<StopDto>
            {
                new StopDto { Id = stops[0].Id, Name = stops[0].Name },
                new StopDto { Id = stops[1].Id, Name = stops[1].Name }
            };

            _mockStopRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(stops);

            _mockMapper.Setup(m => m.Map<IEnumerable<StopDto>>(It.IsAny<IEnumerable<Stop>>()))
                .Returns(stopDtos);

            // Act
            var result = await _stopService.GetStopsWithNoRoutes();

            // Assert
            Assert.Equal(2, result.Count()); // Ensure only 2 stops are returned, as 1 has a route
        }

        [Fact]
        public async Task GetStopsWithNoRoutes_ShouldReturnEmptyList_WhenNoStopsHaveNoRoutes()
        {
            // Arrange
            var stops = new List<Stop>
            {
                new Stop { Id = Guid.NewGuid(), Name = "Stop 1", RouteId = Guid.NewGuid() },
                new Stop { Id = Guid.NewGuid(), Name = "Stop 2", RouteId = Guid.NewGuid() }
            };

            _mockStopRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(stops);

            // Act
            var result = await _stopService.GetStopsWithNoRoutes();

            // Assert
            Assert.Empty(result); // Ensure no stops are returned when all have routes
        }

        [Fact]
        public async Task GetStopsWithNoRoutes_ShouldReturnEmptyList_WhenNoStopsExist()
        {
            // Arrange
            _mockStopRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Stop>()); // Simulate empty repository

            // Act
            var result = await _stopService.GetStopsWithNoRoutes();

            // Assert
            Assert.Empty(result); // Ensure an empty list is returned when no stops exist
        }

        [Fact]
        public async Task GetStopsWithNoRoutes_ShouldReturnEmptyList_WhenStopsIsNull()
        {
            // Arrange
            _mockStopRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync((List<Stop>)null); // Simulate null repository response

            // Act
            var result = await _stopService.GetStopsWithNoRoutes();

            // Assert
            Assert.Empty(result); // Ensure an empty list is returned when repository returns null
        }

        [Fact]
        public async Task GetStopsWithNoRoutes_ShouldReturnEmptyList_WhenMappingReturnsNull()
        {
            // Arrange
            var stops = new List<Stop>
            {
                new Stop { Id = Guid.NewGuid(), Name = "Stop 1", RouteId = null }
            };

            _mockStopRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(stops);

            _mockMapper.Setup(m => m.Map<IEnumerable<StopDto>>(It.IsAny<IEnumerable<Stop>>()))
                .Returns((IEnumerable<StopDto>)null); // Simulate mapping failure

            // Act
            var result = await _stopService.GetStopsWithNoRoutes();

            // Assert
            Assert.Empty(result); // Ensure an empty list is returned when mapping fails
        }
    }
}

