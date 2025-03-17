using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MSLogistics.Application.Repositories.IRouteRepository;
using MSLogistics.Application.Repositories.IStopRepository;
using MSLogistics.Application.Repositories.IVehicleRepository;
using MSLogistics.Application.Services.RouteService;
using MSLogistics.Application.ValueObjects.DTOs.Route;
using MSLogistics.Application.ValueObjects.DTOs.Stop;
using MSLogistics.Application.ValueObjects.DTOs.Vehicle;
using MSLogistics.Domain;
using Xunit;

namespace MSLogistics.UnitTests.ServiceTests.RouteServiceTests
{
	public class UpdateRoutesTests
	{
        private readonly RouteService _routeService;
        private readonly Mock<IRouteRepository> _mockRouteRepository;
        private readonly Mock<IVehicleRepository> _mockVehicleRepository;
        private readonly Mock<IStopRepository> _mockStopRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<RouteService>> _mockLogger;

        public UpdateRoutesTests()
        {
            _mockRouteRepository = new Mock<IRouteRepository>();
            _mockVehicleRepository = new Mock<IVehicleRepository>();
            _mockStopRepository = new Mock<IStopRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<RouteService>>();

            _routeService = new RouteService(_mockRouteRepository.Object, _mockVehicleRepository.Object, _mockStopRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task UpdateRoutes_ShouldReturnFalse_WhenRoutesListIsNull()
        {
            // Act
            var result = await _routeService.UpdateRoutes(null);

            // Assert
            Assert.False(result); // Ensure the result is false when routes list is null
        }

        [Fact]
        public async Task UpdateRoutes_ShouldReturnFalse_WhenRoutesListIsEmpty()
        {
            // Act
            var result = await _routeService.UpdateRoutes(new List<RouteDto>());

            // Assert
            Assert.False(result); // Ensure the result is false when routes list is empty
        }


        [Fact]
        public async Task UpdateRoutes_ShouldReturnFalse_WhenNoValidRoutesToUpdate()
        {
            // Arrange
            var routeDtos = new List<RouteDto>
            {
                new RouteDto { Id = Guid.NewGuid(), Name = "NonExistentRoute" }
            };

            _mockRouteRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Route)null);

            _mockRouteRepository.Setup(repo => repo.UpdateRangeAsync(It.IsAny<IEnumerable<Route>>()))
                .ReturnsAsync(false); // Simulate update failure

            // Act
            var result = await _routeService.UpdateRoutes(routeDtos);

            // Assert
            Assert.False(result); // Ensure the result is false when no valid routes were updated
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No valid routes found to update.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateRoutes_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var routeDtos = new List<RouteDto>
            {
                new RouteDto { Id = Guid.NewGuid(), Name = "Route A", Stops = new List<StopDto>() }
            };

            var existingRoute = new Route { Id = routeDtos[0].Id, Name = "Route A", Stops = new List<Stop>() };

            _mockRouteRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRoute);

            _mockRouteRepository.Setup(repo => repo.UpdateRangeAsync(It.IsAny<IEnumerable<Route>>()))
                .ThrowsAsync(new Exception("Repository error"));

            // Act
            var result = await _routeService.UpdateRoutes(routeDtos);

            // Assert
            Assert.False(result); // Ensure the result is false due to exception
            _mockLogger.Verify(
               l => l.Log(
                   LogLevel.Error,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Exception occurred while updating routes.")),
                   It.IsAny<Exception>(),
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()
               ),
               Times.Once
           );
        }

        [Fact]
        public async Task UpdateRoutes_ShouldReturnTrue_WhenRoutesAreUpdatedSuccessfully()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var routeId = Guid.NewGuid();

            var routeDtos = new List<RouteDto>
            {
                new RouteDto
                {
                    Id = routeId,
                    Name = "Updated Route A",
                    Vehicle = new VehicleDto { Id = vehicleId, VehicleModel = "Updated Model A" },
                    Stops = new List<StopDto>() // Ensure Stops is initialized
                }
            };

            var existingRoute = new Route
            {
                Id = routeId,
                Name = "Old Route A",
                VehicleId = vehicleId,
                Stops = new List<Stop>()
            };

            var vehicle = new Vehicle { Id = vehicleId, VehicleModel = "Old Model A" };

            _mockRouteRepository.Setup(repo => repo.GetByIdAsync(routeId))
                .ReturnsAsync(existingRoute);

            _mockVehicleRepository.Setup(repo => repo.GetByIdAsync(vehicleId))
                .ReturnsAsync(vehicle); // Mock fetching the vehicle

            _mockMapper.Setup(m => m.Map(It.IsAny<RouteDto>(), It.IsAny<Route>()))
                .Callback<RouteDto, Route>((routeDto, route) =>
                {
                    route.Name = routeDto.Name;
                    route.VehicleId = routeDto.Vehicle.Id;
                });

            _mockRouteRepository.Setup(repo => repo.UpdateRangeAsync(It.IsAny<IEnumerable<Route>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _routeService.UpdateRoutes(routeDtos);

            // Assert
            Assert.True(result); // Ensure the result is true when routes are updated

            _mockRouteRepository.Verify(repo => repo.UpdateRangeAsync(It.Is<IEnumerable<Route>>(r =>
                r.Count() == 1 &&
                r.First().Name == "Updated Route A" &&
                r.First().VehicleId == vehicleId)), Times.Once);
        }


        [Fact]
        public async Task UpdateRoutes_ShouldReturnFalse_WhenExistingRouteNotFound()
        {
            // Arrange
            var routeDtos = new List<RouteDto>
            {
                new RouteDto { Id = Guid.NewGuid(), Name = "Route A", Stops = new List<StopDto>() }
            };

            _mockRouteRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Route)null); // Simulate route not found

            // Act
            var result = await _routeService.UpdateRoutes(routeDtos);

            // Assert
            Assert.False(result); // Ensure the result is false when route does not exist
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

    }
}

