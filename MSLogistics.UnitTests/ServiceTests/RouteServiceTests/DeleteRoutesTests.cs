using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MSLogistics.Application.Repositories.IRouteRepository;
using MSLogistics.Application.Repositories.IStopRepository;
using MSLogistics.Application.Repositories.IVehicleRepository;
using MSLogistics.Application.Services.RouteService;
using MSLogistics.Domain;
using Xunit;

namespace MSLogistics.UnitTests.ServiceTests.RouteServiceTests
{
	public class DeleteRoutesTests
	{
        private readonly RouteService _routeService;
        private readonly Mock<IRouteRepository> _mockRouteRepository;
        private readonly Mock<IVehicleRepository> _mockVehicleRepository;
        private readonly Mock<IStopRepository> _mockStopRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<RouteService>> _mockLogger;

        public DeleteRoutesTests()
        {
            _mockRouteRepository = new Mock<IRouteRepository>();
            _mockVehicleRepository = new Mock<IVehicleRepository>();
            _mockStopRepository = new Mock<IStopRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<RouteService>>();

            _routeService = new RouteService(_mockRouteRepository.Object, _mockVehicleRepository.Object, _mockStopRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task DeleteRoutes_ShouldReturnTrue_WhenRoutesAreDeletedSuccessfully()
        {
            // Arrange
            var routeId1 = Guid.NewGuid();
            var routeId2 = Guid.NewGuid();
            var routeIds = new List<Guid> { routeId1, routeId2 };

            var routes = new List<Route>
            {
                new Route { Id = routeId1, Name = "Route 1" },
                new Route { Id = routeId2, Name = "Route 2" }
            };

            _mockRouteRepository
                .Setup(repo => repo.GetByIdAsync(routeId1))
                .ReturnsAsync(routes.First());
            _mockRouteRepository
                .Setup(repo => repo.GetByIdAsync(routeId2))
                .ReturnsAsync(routes.Last());
            _mockRouteRepository
                .Setup(repo => repo.DeleteRangeAsync(It.IsAny<ICollection<Guid>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _routeService.DeleteRoutes(routeIds);

            // Assert
            Assert.True(result); // Ensure the result is true indicating successful deletion
            _mockRouteRepository.Verify(repo => repo.DeleteRangeAsync(It.Is<ICollection<Guid>>(ids => ids.Count == 2)), Times.Once);
        }

        [Fact]
        public async Task DeleteRoutes_ShouldReturnFalse_WhenNoIdsProvided()
        {
            // Arrange
            var routeIds = new List<Guid>();

            // Act
            var result = await _routeService.DeleteRoutes(routeIds);

            // Assert
            Assert.False(result); // Ensure that the result is false when no IDs are provided
        }

        [Fact]
        public async Task DeleteRoutes_ShouldReturnFalse_WhenRouteNotFound()
        {
            // Arrange
            var routeId1 = Guid.NewGuid();
            var routeId2 = Guid.NewGuid();
            var routeIds = new List<Guid> { routeId1, routeId2 };

            _mockRouteRepository
                .Setup(repo => repo.GetByIdAsync(routeId1))
                .ReturnsAsync((Route?)null); // Route not found
            _mockRouteRepository
                .Setup(repo => repo.GetByIdAsync(routeId2))
                .ReturnsAsync(new Route { Id = routeId2, Name = "Route 2" });

            // Act
            var result = await _routeService.DeleteRoutes(routeIds);

            // Assert
            Assert.False(result); // Ensure that the result is false when a route is not found
            _mockLogger.Verify(
              l => l.Log(
                  LogLevel.Error,
                  It.IsAny<EventId>(),
                  It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Route with ID")),
                  It.IsAny<Exception>(),
                  It.IsAny<Func<It.IsAnyType, Exception, string>>()
              ),
              Times.Once
          );
        }

        [Fact]
        public async Task DeleteRoutes_ShouldReturnFalse_WhenDeleteFails()
        {
            // Arrange
            var routeId1 = Guid.NewGuid();
            var routeId2 = Guid.NewGuid();
            var routeIds = new List<Guid> { routeId1, routeId2 };

            var routes = new List<Route>
            {
                new Route { Id = routeId1, Name = "Route 1" },
                new Route { Id = routeId2, Name = "Route 2" }
            };

            _mockRouteRepository
                .Setup(repo => repo.GetByIdAsync(routeId1))
                .ReturnsAsync(routes.First());
            _mockRouteRepository
                .Setup(repo => repo.GetByIdAsync(routeId2))
                .ReturnsAsync(routes.Last());
            _mockRouteRepository
                .Setup(repo => repo.DeleteRangeAsync(It.IsAny<ICollection<Guid>>()))
                .ReturnsAsync(false); // Simulate delete failure

            // Act
            var result = await _routeService.DeleteRoutes(routeIds);

            // Assert
            Assert.False(result); // Ensure the result is false when delete fails
        }

        [Fact]
        public async Task DeleteRoutes_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var routeId1 = Guid.NewGuid();
            var routeId2 = Guid.NewGuid();
            var routeIds = new List<Guid> { routeId1, routeId2 };

            _mockRouteRepository
                .Setup(repo => repo.GetByIdAsync(routeId1))
                .ThrowsAsync(new Exception("Repository failure"));
            _mockRouteRepository
                .Setup(repo => repo.GetByIdAsync(routeId2))
                .ReturnsAsync(new Route { Id = routeId2, Name = "Route 2" });

            // Act
            var result = await _routeService.DeleteRoutes(routeIds);

            // Assert
            Assert.False(result); // Ensure the result is false due to exception
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

