using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MSLogistics.Application.Repositories.IRouteRepository;
using MSLogistics.Application.Services.RouteService;
using MSLogistics.Application.ValueObjects.DTOs.Route;
using MSLogistics.Application.ValueObjects.DTOs.Stop;
using MSLogistics.Application.ValueObjects.DTOs.Vehicle;
using MSLogistics.Domain;
using Xunit;

namespace MSLogistics.UnitTests.ServiceTests.RouteServiceTests
{
    public class GetRoutesTests
    {
        private readonly RouteService _routeService;
        private readonly Mock<IRouteRepository> _mockRouteRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<RouteService>> _mockLogger;

        public GetRoutesTests()
        {
            _mockRouteRepository = new Mock<IRouteRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<RouteService>>();

            _routeService = new RouteService(_mockRouteRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetRoutes_ShouldReturnEmptyList_WhenNoRoutesExist()
        {
            // Arrange
            _mockRouteRepository
                .Setup(repo => repo.GetAllWithIncludesAsync(It.IsAny<Expression<Func<Route, object>>[]>()))
                .ReturnsAsync(new List<Route>());

            _mockMapper
                .Setup(mapper => mapper.Map<IEnumerable<RouteDto>>(It.IsAny<IEnumerable<Route>>()))
                .Returns(new List<RouteDto>());

            // Act
            var result = await _routeService.GetRoutes();

            // Assert
            Assert.NotNull(result); // Ensure result is not null
            Assert.Empty(result);  // Ensure the list is empty
        }

        [Fact]
        public async Task GetRoutes_ShouldReturnEmptyList_WhenRepositoryReturnsNull()
        {
            // Arrange
            _mockRouteRepository
                .Setup(repo => repo.GetAllWithIncludesAsync(It.IsAny<Expression<Func<Route, object>>[]>()))
                .ReturnsAsync((IEnumerable<Route>?)null);

            _mockMapper
                .Setup(mapper => mapper.Map<IEnumerable<RouteDto>>(null))
                .Returns((IEnumerable<RouteDto>?)null);

            // Act
            var result = await _routeService.GetRoutes();

            // Assert
            Assert.NotNull(result); // Ensure result is not null
            Assert.Empty(result);  // Ensure the list is empty
        }

        [Fact]
        public async Task GetRoutes_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            _mockRouteRepository
                .Setup(repo => repo.GetAllWithIncludesAsync(It.IsAny<Expression<Func<Route, object>>[]>()))
                .ThrowsAsync(new Exception("Repository failure"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _routeService.GetRoutes());
        }

        [Fact]
        public async Task GetRoutes_ShouldMapVehicleAndStopsCorrectly()
        {
            // Arrange
            var routeId = Guid.NewGuid();
            var vehicleId = Guid.NewGuid();
            var stopId = Guid.NewGuid();

            var routes = new List<Route>
            {
                new Route
                {
                    Id = routeId,
                    Name = "Route A",
                    VehicleId = vehicleId,
                    Vehicle = new Vehicle
                    {
                        Id = vehicleId,
                        RegistrationNumber = "ABC123",
                        VehicleMake = "Tesla",
                        VehicleModel = "Model X",
                        LoadCapacity = 1500
                    },
                    Stops = new List<Stop>
                    {
                        new Stop
                        {
                            Id = stopId,
                            Name = "Stop 1",
                            CustomerId = Guid.NewGuid()
                        }
                    }
                }
            };

            var routeDtos = new List<RouteDto>
            {
                new RouteDto
                {
                    Id = routeId,
                    Name = "Route A",
                    Vehicle = new VehicleDto
                    {
                        Id = vehicleId,
                        RegistrationNumber = "ABC123",
                        VehicleMake = "Tesla",
                        VehicleModel = "Model X",
                        LoadCapacity = 1500
                    },
                    Stops = new List<StopDto>
                    {
                        new StopDto
                        {
                            Id = stopId,
                            Name = "Stop 1",
                            CustomerId = Guid.NewGuid()
                        }
                    }
                }
            };

            _mockRouteRepository
                .Setup(repo => repo.GetAllWithIncludesAsync(It.IsAny<Expression<Func<Route, object>>[]>()))
                .ReturnsAsync(routes);

            _mockMapper
                .Setup(mapper => mapper.Map<IEnumerable<RouteDto>>(routes))
                .Returns(routeDtos);

            // Act
            var result = await _routeService.GetRoutes();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Ensure one route is returned
            var route = result.First();
            Assert.NotNull(route.Vehicle);
            Assert.NotEmpty(route.Stops);
            Assert.Equal("Route A", route.Name);
            Assert.Equal("Tesla", route.Vehicle.VehicleMake);
        }

        [Fact]
        public async Task GetRoutes_ShouldHandleNullMappingGracefully()
        {
            // Arrange
            var routes = new List<Route>
            {
                new Route
                {
                    Id = Guid.NewGuid(),
                    Name = "Route A",
                    VehicleId = Guid.NewGuid(),
                    Vehicle = null, // Vehicle is null
                    Stops = new List<Stop>()
                }
            };

            var routeDtos = new List<RouteDto>
            {
                new RouteDto
                {
                    Id = routes.First().Id,
                    Name = "Route A",
                    Vehicle = null, // VehicleDto is also null
                    Stops = new List<StopDto>()
                }
            };

            _mockRouteRepository
                .Setup(repo => repo.GetAllWithIncludesAsync(It.IsAny<Expression<Func<Route, object>>[]>()))
                .ReturnsAsync(routes);

            _mockMapper
                .Setup(mapper => mapper.Map<IEnumerable<RouteDto>>(routes))
                .Returns(routeDtos);

            // Act
            var result = await _routeService.GetRoutes();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Null(result.First().Vehicle); // Ensure Vehicle is null as expected
            Assert.Empty(result.First().Stops); // Ensure Stops list is empty
        }
    }
}

