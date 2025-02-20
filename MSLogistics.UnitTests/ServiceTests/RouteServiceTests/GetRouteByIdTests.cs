using System.Linq.Expressions;
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
	public class GetRouteByIdTests
	{
        private readonly RouteService _routeService;
        private readonly Mock<IRouteRepository> _mockRouteRepository;
        private readonly Mock<IVehicleRepository> _mockVehicleRepository;
        private readonly Mock<IStopRepository> _mockStopRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<RouteService>> _mockLogger;

        public GetRouteByIdTests()
        {
            _mockRouteRepository = new Mock<IRouteRepository>();
            _mockVehicleRepository = new Mock<IVehicleRepository>();
            _mockStopRepository = new Mock<IStopRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<RouteService>>();

            _routeService = new RouteService(_mockRouteRepository.Object, _mockVehicleRepository.Object, _mockStopRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetRouteById_ShouldReturnRoute_WhenRouteExists()
        {
            // Arrange
            var routeId = Guid.NewGuid();
            var vehicleId = Guid.NewGuid();
            var stopId = Guid.NewGuid();

            var route = new Route
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
            };

            var routeDto = new RouteDto
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
            };

            _mockRouteRepository
                .Setup(repo => repo.GetRoutesByIdWithIncludesAsync(routeId, It.IsAny<Expression<Func<Route, object>>[]>()))
                .ReturnsAsync(route);

            _mockMapper
                .Setup(mapper => mapper.Map<RouteDto>(route))
                .Returns(routeDto);

            // Act
            var result = await _routeService.GetRouteById(routeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(routeId, result.Id);
            Assert.Equal("Route A", result.Name);
            Assert.NotNull(result.Vehicle);
            Assert.NotEmpty(result.Stops);
            Assert.Equal("Tesla", result.Vehicle.VehicleMake);
        }

        [Fact]
        public async Task GetRouteById_ShouldReturnNewRouteDto_WhenRouteNotFound()
        {
            // Arrange
            var routeId = Guid.NewGuid();

            _mockRouteRepository
                .Setup(repo => repo.GetRoutesByIdWithIncludesAsync(routeId, It.IsAny<Expression<Func<Route, object>>[]>()))
                .ReturnsAsync((Route?)null);

            _mockMapper
                .Setup(mapper => mapper.Map<RouteDto>(null))
                .Returns(new RouteDto());

            // Act
            var result = await _routeService.GetRouteById(routeId);

            // Assert
            Assert.NotNull(result); // Ensure a result is returned
            Assert.IsType<RouteDto>(result); // Ensure it's of type RouteDto
        }

        [Fact]
        public async Task GetRouteById_ShouldHandleMappingNullGracefully()
        {
            // Arrange
            var routeId = Guid.NewGuid();

            var route = new Route
            {
                Id = routeId,
                Name = "Route A",
                Vehicle = null, // Vehicle is null
                Stops = new List<Stop>() // Empty stops
            };

            _mockRouteRepository
                .Setup(repo => repo.GetRoutesByIdWithIncludesAsync(routeId, It.IsAny<Expression<Func<Route, object>>[]>()))
                .ReturnsAsync(route);

            _mockMapper
                .Setup(mapper => mapper.Map<RouteDto>(route))
                .Returns((RouteDto?)null); // Simulating null mapping

            // Act
            var result = await _routeService.GetRouteById(routeId);

            // Assert
            Assert.NotNull(result); // Ensure result is returned even if mapping is null
            Assert.IsType<RouteDto>(result); // Ensure it's of type RouteDto
            Assert.Null(result.Vehicle); // Ensure Vehicle is null
            Assert.Empty(result.Stops); // Ensure Stops list is empty
        }

        [Fact]
        public async Task GetRouteById_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var routeId = Guid.NewGuid();

            _mockRouteRepository
                .Setup(repo => repo.GetRoutesByIdWithIncludesAsync(routeId, It.IsAny<Expression<Func<Route, object>>[]>()))
                .ThrowsAsync(new Exception("Repository failure"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _routeService.GetRouteById(routeId));
        }
    }
}

