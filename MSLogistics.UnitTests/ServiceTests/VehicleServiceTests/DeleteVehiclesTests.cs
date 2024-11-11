using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MSLogistics.Application.Repositories.IVehicleRepository;
using MSLogistics.Application.Services.VehicleService;
using MSLogistics.Domain;
using Xunit;

namespace MSLogistics.UnitTests.ServiceTests.VehicleServiceTests
{
    public class DeleteVehiclesTests
    {
        private readonly VehicleService _vehicleService;
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger> _loggerMock;

        public DeleteVehiclesTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger>();
            _vehicleService = new VehicleService(_vehicleRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task DeleteVehicles_ReturnsFalse_WhenIdsAreNull()
        {
            // Act
            var result = await _vehicleService.DeleteVehicles(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteVehicles_ReturnsFalse_WhenIdsAreEmpty()
        {
            // Act
            var result = await _vehicleService.DeleteVehicles(new List<Guid>());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteVehicles_ReturnsFalseAndLogsError_WhenVehicleNotFound()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };
            _vehicleRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Vehicle)null);

            // Act
            var result = await _vehicleService.DeleteVehicles(ids);

            // Assert
            Assert.False(result);
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Vehicle with ID {ids[0]} not found in the repository.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteVehicles_DeletesVehiclesAndReturnsTrue_WhenAllVehiclesFound()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var vehicles = ids.Select(id => new Vehicle { Id = id }).ToList();
            _vehicleRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .Returns<Guid>(id => Task.FromResult(vehicles.FirstOrDefault(v => v.Id == id)));
            _vehicleRepositoryMock.Setup(repo => repo.DeleteRangeAsync(It.IsAny<ICollection<Guid>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _vehicleService.DeleteVehicles(ids);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteVehicles_LogsErrorAndReturnsFalse_WhenExceptionThrown()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };
            _vehicleRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _vehicleService.DeleteVehicles(ids);

            // Assert
            Assert.False(result);
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Exception was thrown while deleting a range of records of the MSLogistics.Domain.Vehicle type.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
}

