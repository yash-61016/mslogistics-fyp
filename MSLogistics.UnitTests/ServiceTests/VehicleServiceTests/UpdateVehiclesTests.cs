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
	public class UpdateVehiclesTests
	{
        private readonly VehicleService _vehicleService;
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<VehicleService>> _loggerMock;

        public UpdateVehiclesTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<VehicleService>>();
            _vehicleService = new VehicleService(_vehicleRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task UpdateVehicles_ReturnsFalse_WhenVehicleListIsNull()
        {
            // Act
            var result = await _vehicleService.UpdateVehicles(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateVehicles_ReturnsFalse_WhenVehicleListIsEmpty()
        {
            // Act
            var result = await _vehicleService.UpdateVehicles(new List<VehicleDto>());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateVehicles_LogsErrorAndSkips_WhenVehicleNotFound()
        {
            // Arrange
            var vehicleDtos = new List<VehicleDto> { new VehicleDto { Id = Guid.NewGuid() } };
            _vehicleRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Vehicle)null);

            // Act
            var result = await _vehicleService.UpdateVehicles(vehicleDtos);

            // Assert
            Assert.False(result);
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Vehicle with ID {vehicleDtos[0].Id} not found for updating.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateVehicles_ReturnsTrue_WhenVehiclesSuccessfullyUpdated()
        {
            // Arrange
            var vehicleDtos = new List<VehicleDto>
            {
                new VehicleDto { Id = Guid.NewGuid(), RegisterationNumber = "ABC123" },
                new VehicleDto { Id = Guid.NewGuid(), RegisterationNumber = "DEF456" }
            };
            var vehicles = vehicleDtos.Select(dto => new Vehicle { Id = dto.Id }).ToList();

            _vehicleRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .Returns<Guid>(id => Task.FromResult(vehicles.FirstOrDefault(v => v.Id == id)));
            _vehicleRepositoryMock.Setup(repo => repo.UpdateRangeAsync(It.IsAny<IEnumerable<Vehicle>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _vehicleService.UpdateVehicles(vehicleDtos);

            // Assert
            Assert.True(result);
            _vehicleRepositoryMock.Verify(repo => repo.UpdateRangeAsync(It.Is<IEnumerable<Vehicle>>(v => v.Count() == vehicles.Count)), Times.Once);
        }

        [Fact]
        public async Task UpdateVehicles_LogsErrorAndReturnsFalse_WhenExceptionThrown()
        {
            // Arrange
            var vehicleDtos = new List<VehicleDto> { new VehicleDto { Id = Guid.NewGuid() } };
            _vehicleRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _vehicleService.UpdateVehicles(vehicleDtos);

            // Assert
            Assert.False(result);
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Exception was thrown while updating a range of records of the MSLogistics.Domain.Vehicle type.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
}

