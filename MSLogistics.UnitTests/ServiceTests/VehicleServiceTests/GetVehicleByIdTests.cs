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
	public class GetVehicleByIdTests
	{
        private readonly VehicleService _vehicleService;
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public GetVehicleByIdTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _mapperMock = new Mock<IMapper>();
            _vehicleService = new VehicleService(_vehicleRepositoryMock.Object, _mapperMock.Object, Mock.Of<ILogger>());
        }

        [Fact]
        public async Task GetVehicleById_ReturnsMappedVehicleDto_WhenVehicleExists()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var vehicle = new Vehicle
            {
                Id = vehicleId,
                RegisterationNumber = "ABC123",
                LoadCapacity = 1000,
                VehicleModel = "Model X",
                VehicleMake = "Make Y"
            };

            var vehicleDto = new VehicleDto
            {
                Id = vehicleId,
                RegisterationNumber = "ABC123",
                LoadCapacity = 1000,
                VehicleModel = "Model X",
                VehicleMake = "Make Y"
            };

            _vehicleRepositoryMock.Setup(repo => repo.GetByIdAsync(vehicleId)).ReturnsAsync(vehicle);
            _mapperMock.Setup(m => m.Map<VehicleDto>(vehicle)).Returns(vehicleDto);

            // Act
            var result = await _vehicleService.GetVehicleById(vehicleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vehicleDto.Id, result.Id);
            Assert.Equal(vehicleDto.RegisterationNumber, result.RegisterationNumber);
            Assert.Equal(vehicleDto.LoadCapacity, result.LoadCapacity);
            Assert.Equal(vehicleDto.VehicleModel, result.VehicleModel);
            Assert.Equal(vehicleDto.VehicleMake, result.VehicleMake);
        }

        [Fact]
        public async Task GetVehicleById_ReturnsNewVehicleDto_WhenVehicleNotFound()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();

            _vehicleRepositoryMock.Setup(repo => repo.GetByIdAsync(vehicleId)).ReturnsAsync((Vehicle)null);
            _mapperMock.Setup(m => m.Map<VehicleDto>(It.IsAny<Vehicle>())).Returns(new VehicleDto());

            // Act
            var result = await _vehicleService.GetVehicleById(vehicleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Guid.Empty, result.Id);
            Assert.Null(result.RegisterationNumber);
            Assert.Equal(0, result.LoadCapacity);
            Assert.Null(result.VehicleModel);
            Assert.Null(result.VehicleMake);
        }

        [Fact]
        public async Task GetVehicleById_CallsRepositoryWithCorrectId()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();

            _vehicleRepositoryMock.Setup(repo => repo.GetByIdAsync(vehicleId)).ReturnsAsync(new Vehicle());
            _mapperMock.Setup(m => m.Map<VehicleDto>(It.IsAny<Vehicle>())).Returns(new VehicleDto());

            // Act
            await _vehicleService.GetVehicleById(vehicleId);

            // Assert
            _vehicleRepositoryMock.Verify(repo => repo.GetByIdAsync(vehicleId), Times.Once);
        }

        [Fact]
        public async Task GetVehicleById_ReturnsMappedDto_WhenRepositoryReturnsEmptyVehicle()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var emptyVehicle = new Vehicle();

            var mappedVehicleDto = new VehicleDto
            {
                Id = Guid.Empty,
                RegisterationNumber = null,
                LoadCapacity = 0,
                VehicleModel = null,
                VehicleMake = null
            };

            _vehicleRepositoryMock.Setup(repo => repo.GetByIdAsync(vehicleId)).ReturnsAsync(emptyVehicle);
            _mapperMock.Setup(m => m.Map<VehicleDto>(emptyVehicle)).Returns(mappedVehicleDto);

            // Act
            var result = await _vehicleService.GetVehicleById(vehicleId);

            // Assert
            Assert.Equal(Guid.Empty, result.Id);
            Assert.Null(result.RegisterationNumber);
            Assert.Equal(0, result.LoadCapacity);
            Assert.Null(result.VehicleModel);
            Assert.Null(result.VehicleMake);
        }
    }
}

