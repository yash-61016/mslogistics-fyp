using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MSLogistics.Application.Repositories.IVehicleRepository;
using MSLogistics.Application.Services.VehicleService;
using MSLogistics.Application.ValueObjects.DTOs.Vehicle;
using MSLogistics.Domain;
using Xunit;

namespace MSLogistics.UnitTests.ServiceTests.VehicleServiceTests
{
	public class AddVehiclesTests
	{
        private readonly VehicleService _vehicleService;
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<VehicleService>> _loggerMock;

        public AddVehiclesTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<VehicleService>>();
            _vehicleService = new VehicleService(_vehicleRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddVehicles_ReturnsFalse_WhenVehicleListIsNull()
        {
            // Arrange
            IEnumerable<VehicleDto> vehicleList = null;

            // Act
            var result = await _vehicleService.AddVehicles(vehicleList);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddVehicles_ReturnsFalse_WhenVehicleListIsEmpty()
        {
            // Arrange
            var vehicleList = new List<VehicleDto>();

            // Act
            var result = await _vehicleService.AddVehicles(vehicleList);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddVehicles_ReturnsTrue_WhenVehiclesAddedSuccessfully()
        {
            // Arrange
            var vehicleList = new List<VehicleDto>
            {
                new VehicleDto { RegisterationNumber = "ABC123", LoadCapacity = 1000, VehicleModel = "Model X", VehicleMake = "Make Y" }
            };

            var vehicleEntities = new List<Vehicle>
            {
                new Vehicle { RegisterationNumber = "ABC123", LoadCapacity = 1000, VehicleModel = "Model X", VehicleMake = "Make Y" }
            };

            _mapperMock.Setup(m => m.Map<IEnumerable<Vehicle>>(vehicleList)).Returns(vehicleEntities);
            _vehicleRepositoryMock.Setup(repo => repo.AddRangeAsync(vehicleEntities)).ReturnsAsync(true);

            // Act
            var result = await _vehicleService.AddVehicles(vehicleList);

            // Assert
            Assert.True(result);
            _vehicleRepositoryMock.Verify(repo => repo.AddRangeAsync(vehicleEntities), Times.Once);
        }

        [Fact]
        public async Task AddVehicles_AssignsNewIdsToEachVehicle()
        {
            // Arrange
            var vehicleList = new List<VehicleDto>
            {
                new VehicleDto { RegisterationNumber = "ABC123", LoadCapacity = 1000, VehicleModel = "Model X", VehicleMake = "Make Y" }
            };

            var vehicleEntities = new List<Vehicle>
            {
                new Vehicle { RegisterationNumber = "ABC123", LoadCapacity = 1000, VehicleModel = "Model X", VehicleMake = "Make Y" }
            };

            _mapperMock.Setup(m => m.Map<IEnumerable<Vehicle>>(vehicleList)).Returns(vehicleEntities);
            _vehicleRepositoryMock.Setup(repo => repo.AddRangeAsync(vehicleEntities)).ReturnsAsync(true);

            // Act
            await _vehicleService.AddVehicles(vehicleList);

            // Assert
            foreach (var vehicle in vehicleEntities)
            {
                Assert.NotEqual(Guid.Empty, vehicle.Id);
            }
        }

        [Fact]
        public async Task AddVehicles_LogsErrorAndReturnsFalse_WhenExceptionThrown()
        {
            // Arrange
            var vehicleList = new List<VehicleDto>
            {
                new VehicleDto { RegisterationNumber = "ABC123", LoadCapacity = 1000, VehicleModel = "Model X", VehicleMake = "Make Y" }
            };

            var vehicleEntities = new List<Vehicle>
            {
                new Vehicle { RegisterationNumber = "ABC123", LoadCapacity = 1000, VehicleModel = "Model X", VehicleMake = "Make Y" }
            };

            _mapperMock.Setup(m => m.Map<IEnumerable<Vehicle>>(vehicleList)).Returns(vehicleEntities);
            _vehicleRepositoryMock.Setup(repo => repo.AddRangeAsync(vehicleEntities)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _vehicleService.AddVehicles(vehicleList);

            // Assert
            Assert.False(result);
            _loggerMock.Verify(
                 l => l.Log(
                     LogLevel.Error,
                     It.IsAny<EventId>(),
                     It.Is<It.IsAnyType>((v, t) =>
                         v.ToString().Contains("Exception was thrown while inserting a range of records of the MSLogistics.Domain.Vehicle type.")),
                     It.IsAny<Exception>(),
                     It.IsAny<Func<It.IsAnyType, Exception, string>>()
                 ),
                 Times.Once
             );
        }
    }
}

