using System;
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
	public class GetVehicles
	{
        private readonly VehicleService _vehicleService;
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public GetVehicles()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _mapperMock = new Mock<IMapper>();

            _vehicleService = new VehicleService(_vehicleRepositoryMock.Object, _mapperMock.Object, Mock.Of<ILogger>());
        }

        [Fact]
        public async Task GetVehicles_ReturnsMappedVehicleDtos_WhenVehiclesExist()
        {
            // Arrange
            var vehicles = new List<Vehicle>
            {
                new Vehicle { Id = Guid.NewGuid(), RegisterationNumber = "ABC123", LoadCapacity = 1000, VehicleModel = "Model X", VehicleMake = "Make Y" },
                new Vehicle { Id = Guid.NewGuid(), RegisterationNumber = "XYZ789", LoadCapacity = 2000, VehicleModel = "Model Z", VehicleMake = "Make W" }
            };

            var vehicleDtos = new List<VehicleDto>
            {
                new VehicleDto { Id = vehicles[0].Id, RegisterationNumber = "ABC123", LoadCapacity = 1000, VehicleModel = "Model X", VehicleMake = "Make Y" },
                new VehicleDto { Id = vehicles[1].Id, RegisterationNumber = "XYZ789", LoadCapacity = 2000, VehicleModel = "Model Z", VehicleMake = "Make W" }
            };

            _vehicleRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(vehicles);
            _mapperMock.Setup(m => m.Map<IEnumerable<VehicleDto>>(vehicles)).Returns(vehicleDtos);

            // Act
            var result = await _vehicleService.GetVehicles();

            // Assert
            Assert.Equal(vehicleDtos.Count, result.Count());
            Assert.Equal(vehicleDtos, result);
        }

        [Fact]
        public async Task GetVehicles_ReturnsEmptyList_WhenNoVehiclesExist()
        {
            // Arrange
            _vehicleRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Vehicle>());
            _mapperMock.Setup(m => m.Map<IEnumerable<VehicleDto>>(It.IsAny<IEnumerable<Vehicle>>())).Returns(new List<VehicleDto>());

            // Act
            var result = await _vehicleService.GetVehicles();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetVehicles_ReturnsEmptyList_WhenRepositoryReturnsNull()
        {
            // Arrange
            _vehicleRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync((IEnumerable<Vehicle>)null);
            _mapperMock.Setup(m => m.Map<IEnumerable<VehicleDto>>(It.IsAny<IEnumerable<Vehicle>>())).Returns(new List<VehicleDto>());

            // Act
            var result = await _vehicleService.GetVehicles();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetVehicles_MapsVehiclesCorrectly()
        {
            // Arrange
            var vehicles = new List<Vehicle>
            {
                new Vehicle { Id = Guid.NewGuid(), RegisterationNumber = "ABC123", LoadCapacity = 1000, VehicleModel = "Model X", VehicleMake = "Make Y" }
            };

            var vehicleDtos = new List<VehicleDto>
            {
                new VehicleDto { Id = vehicles[0].Id, RegisterationNumber = "ABC123", LoadCapacity = 1000, VehicleModel = "Model X", VehicleMake = "Make Y" }
            };

            _vehicleRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(vehicles);
            _mapperMock.Setup(m => m.Map<IEnumerable<VehicleDto>>(vehicles)).Returns(vehicleDtos);

            // Act
            var result = await _vehicleService.GetVehicles();

            // Assert
            Assert.Single(result);
            Assert.Equal(vehicleDtos.First().Id, result.First().Id);
            Assert.Equal(vehicleDtos.First().RegisterationNumber, result.First().RegisterationNumber);
            Assert.Equal(vehicleDtos.First().LoadCapacity, result.First().LoadCapacity);
            Assert.Equal(vehicleDtos.First().VehicleModel, result.First().VehicleModel);
            Assert.Equal(vehicleDtos.First().VehicleMake, result.First().VehicleMake);
        }
    }
}


