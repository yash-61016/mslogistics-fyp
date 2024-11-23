using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MSLogistics.Application.Repositories.IRouteRepository;
using MSLogistics.Application.Services.RouteService;
using MSLogistics.Application.ValueObjects.DTOs.Route;
using MSLogistics.Application.ValueObjects.DTOs.Vehicle;
using MSLogistics.Domain;
using Xunit;

namespace MSLogistics.UnitTests.ServiceTests.RouteServiceTests
{
	public class AddRoutesTests
	{
        private readonly RouteService _routeService;
        private readonly Mock<IRouteRepository> _mockRouteRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<RouteService>> _mockLogger;

        public AddRoutesTests()
        {
            _mockRouteRepository = new Mock<IRouteRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<RouteService>>();

            _routeService = new RouteService(_mockRouteRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AddRoutes_ShouldReturnTrue_WhenRoutesAreAddedSuccessfully()
        {
            // Arrange
            var routesList = new List<RouteDto>
            {
                new RouteDto { Name = "Route A", Vehicle = new VehicleDto { Id = Guid.NewGuid(), VehicleModel = "Model A" } },
                new RouteDto { Name = "Route B", Vehicle = new VehicleDto { Id = Guid.NewGuid(), VehicleModel = "Model B" } }
            };

            var routeEntities = new List<Route>
            {
                new Route { Id = Guid.NewGuid(), Name = "Route A", VehicleId = Guid.NewGuid() },
                new Route { Id = Guid.NewGuid(), Name = "Route B", VehicleId = Guid.NewGuid() }
            };

            // Mock the mapper to return the list of routes
            _mockMapper.Setup(m => m.Map<IEnumerable<Route>>(It.IsAny<IEnumerable<RouteDto>>()))
                .Returns(routeEntities);

            _mockRouteRepository.Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<Route>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _routeService.AddRoutes(routesList);

            // Assert
            Assert.True(result); // Ensure the result is true when routes are added
            _mockRouteRepository.Verify(repo => repo.AddRangeAsync(It.Is<IEnumerable<Route>>(r => r.Count() == 2)), Times.Once);
        }

        [Fact]
        public async Task AddRoutes_ShouldReturnFalse_WhenRoutesListIsNull()
        {
            // Act
            var result = await _routeService.AddRoutes(null);

            // Assert
            Assert.False(result); // Ensure the result is false when routes list is null
        }

        [Fact]
        public async Task AddRoutes_ShouldReturnFalse_WhenRoutesListIsEmpty()
        {
            // Act
            var result = await _routeService.AddRoutes(new List<RouteDto>());

            // Assert
            Assert.False(result); // Ensure the result is false when routes list is empty
        }

        [Fact]
        public async Task AddRoutes_ShouldReturnFalse_WhenAddRangeFails()
        {
            // Arrange
            var routesList = new List<RouteDto>
            {
                new RouteDto { Name = "Route A", Vehicle = new VehicleDto { Id = Guid.NewGuid(), VehicleModel = "Model A" } }
            };

            var routeEntities = new List<Route>
            {
                new Route { Id = Guid.NewGuid(), Name = "Route A", VehicleId = Guid.NewGuid() }
            };

            _mockMapper.Setup(m => m.Map<IEnumerable<Route>>(It.IsAny<IEnumerable<RouteDto>>()))
                .Returns(routeEntities);

            _mockRouteRepository.Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<Route>>()))
                .ReturnsAsync(false); // Simulate add failure

            // Act
            var result = await _routeService.AddRoutes(routesList);

            // Assert
            Assert.False(result); // Ensure the result is false when add fails
        }

        [Fact]
        public async Task AddRoutes_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var routesList = new List<RouteDto>
            {
                new RouteDto { Name = "Route A", Vehicle = new VehicleDto { Id = Guid.NewGuid(), VehicleModel = "Model A" } }
            };

            var routeEntities = new List<Route>
            {
                new Route { Id = Guid.NewGuid(), Name = "Route A", VehicleId = Guid.NewGuid() }
            };

            _mockMapper.Setup(m => m.Map<IEnumerable<Route>>(It.IsAny<IEnumerable<RouteDto>>()))
                .Returns(routeEntities);

            _mockRouteRepository.Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<Route>>()))
                .ThrowsAsync(new Exception("Repository error"));

            // Act
            var result = await _routeService.AddRoutes(routesList);

            // Assert
            Assert.False(result); // Ensure the result is false due to exception
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

