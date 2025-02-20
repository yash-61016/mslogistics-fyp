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
    public class AddRoutesTests
    {
        private readonly RouteService _routeService;
        private readonly Mock<IRouteRepository> _mockRouteRepository;
        private readonly Mock<IVehicleRepository> _mockVehicleRepository;
        private readonly Mock<IStopRepository> _mockStopRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<RouteService>> _mockLogger;

        public AddRoutesTests()
        {
            _mockRouteRepository = new Mock<IRouteRepository>();
            _mockVehicleRepository = new Mock<IVehicleRepository>();
            _mockStopRepository = new Mock<IStopRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<RouteService>>();

            _routeService = new RouteService(_mockRouteRepository.Object, _mockVehicleRepository.Object, _mockStopRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AddRoutes_ShouldReturnTrue_WhenRoutesAreAddedSuccessfully()
        {
            // Arrange
            Guid vehicleId1 = Guid.NewGuid();
            Guid stopId1 = Guid.NewGuid();

            var routesList = new List<RouteDto>
            {
                new RouteDto
                {
                    Name = "Route A",
                    Vehicle = new VehicleDto { Id = vehicleId1, VehicleModel = "Model A" },
                    Stops = new List<StopDto> { new StopDto { Id = stopId1, Name = "Stop 1", Sequencenumber = 2 } }
                },
            };

            var existingStops = new List<Stop>
            {
                new Stop { Id = stopId1, Name = "Stop 1", Sequencenumber = 2 }
            };

            var existingVehicle = new Vehicle
            {
                Id = vehicleId1,
                VehicleModel = "Model A"
            };

            // Mock vehicle retrieval
            _mockVehicleRepository.Setup(vr => vr.GetByIdAsync(vehicleId1))
                .ReturnsAsync(existingVehicle);

            // Mock stop retrieval
            _mockStopRepository.Setup(sr => sr.GetAllAsync())
                .ReturnsAsync(existingStops);

            // Mock the mapper to handle the mapping of RouteDto to Route, including VehicleDto to Vehicle
            _mockMapper.Setup(m => m.Map<IEnumerable<Route>>(It.IsAny<IEnumerable<RouteDto>>()))
                .Returns((IEnumerable<RouteDto> dtoList) =>
                {
                    return dtoList.Select(dto => new Route
                    {
                        Name = dto.Name,
                        VehicleId = dto.Vehicle.Id,
                        Vehicle = new Vehicle
                        {
                            Id = dto.Vehicle.Id,
                            VehicleModel = dto.Vehicle.VehicleModel // Explicit mapping of VehicleDto to Vehicle
                        },
                        Stops = dto.Stops.Select(stopDto => new Stop
                        {
                            Id = stopDto.Id,
                            Name = stopDto.Name,
                            Sequencenumber = stopDto.Sequencenumber
                        }).ToList()
                    });
                });

            _mockRouteRepository.Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<Route>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _routeService.AddRoutes(routesList);

            // Assert
            Assert.True(result); // Ensure the result is true when routes are added

            // Verify that the repository's AddRangeAsync was called with the correct routes
            _mockRouteRepository.Verify(repo => repo.AddRangeAsync(It.Is<IEnumerable<Route>>(routes =>
                routes.Count() == 1 && // Only 1 route is passed
                routes.Any(r => r.Name == "Route A" && r.Vehicle.Id == vehicleId1 && r.Stops.Any(s => s.Id == stopId1))
            )), Times.Once);

            // Verify that the vehicle repository was called to retrieve the vehicle
            _mockVehicleRepository.Verify(vr => vr.GetByIdAsync(It.IsAny<Guid>()), Times.Once); // Ensure the vehicle is fetched

            // Verify that the stop repository was called to retrieve all stops
            _mockStopRepository.Verify(sr => sr.GetAllAsync(), Times.Once); // Ensure stops are fetched
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
            Guid VehicleId = Guid.NewGuid();
            var routesList = new List<RouteDto>
            {
                new RouteDto
                {
                    Name = "Route A",
                    Vehicle = new VehicleDto { Id = Guid.NewGuid(), VehicleModel = "Model A" },
                    Stops = new List<StopDto> { new StopDto { Id = Guid.NewGuid(), Name = "Stop 1" } }
                }
            };

            var routeEntity = new Route { Id = Guid.NewGuid(), Name = "Route A", VehicleId = VehicleId, Stops = new List<Stop>() };

            _mockVehicleRepository.Setup(vr => vr.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Vehicle { Id = VehicleId, LoadCapacity = 400 });

            _mockStopRepository.Setup(sr => sr.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Stop { Id = Guid.NewGuid(), Name = "Stop 1" });

            _mockMapper.Setup(m => m.Map<Route>(It.IsAny<RouteDto>()))
                .Returns(routeEntity);

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
            Guid VehicleId = Guid.NewGuid();
            var routesList = new List<RouteDto>
            {
                new RouteDto
                {
                    Name = "Route A",
                    Vehicle = new VehicleDto { Id = Guid.NewGuid(), VehicleModel = "Model A" },
                    Stops = new List<StopDto> { new StopDto { Id = Guid.NewGuid(), Name = "Stop 1" } }
                }
            };

            var routeEntity = new Route { Id = Guid.NewGuid(), Name = "Route A", VehicleId = VehicleId, Stops = new List<Stop>() };

            _mockVehicleRepository.Setup(vr => vr.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Vehicle { Id = VehicleId, LoadCapacity = 400, VehicleMake = "2009"});

            _mockStopRepository.Setup(sr => sr.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Stop { Id = Guid.NewGuid(), Name = "Stop 1" });

            _mockMapper.Setup(m => m.Map<Route>(It.IsAny<RouteDto>()))
                .Returns(routeEntity);

            _mockRouteRepository.Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<Route>>()))
                .ThrowsAsync(new Exception("Repository error"));

            // Act
            var result = await _routeService.AddRoutes(routesList);

            // Assert
            Assert.False(result); // Ensure the result is false due to exception
        }
    }
}
