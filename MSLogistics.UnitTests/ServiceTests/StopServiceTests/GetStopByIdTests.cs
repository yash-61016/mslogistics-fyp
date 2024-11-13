using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MSLogistics.Application.Repositories.IStopRepository;
using MSLogistics.Application.Services.StopService;
using MSLogistics.Application.ValueObjects.DTOs.Stop;
using MSLogistics.Domain;
using Xunit;

namespace MSLogistics.UnitTests.ServiceTests.StopServiceTests
{
	public class GetStopByIdTests
	{
        private readonly StopService _stopService;
        private readonly Mock<IStopRepository> _mockStopRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<StopService>> _mockLogger;

        public GetStopByIdTests()
        {
            _mockStopRepository = new Mock<IStopRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<StopService>>();

            _stopService = new StopService(_mockStopRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetStopById_ShouldReturnStopDto_WhenStopExists()
        {
            // Arrange
            var stopId = Guid.NewGuid();
            var stop = new Stop { Id = stopId, Name = "Sample Stop", CustomerId = Guid.NewGuid() };
            var expectedStopDto = new StopDto { Id = stopId, Name = "Sample Stop", CustomerId = stop.CustomerId };

            _mockStopRepository.Setup(repo => repo.GetByIdAsync(stopId)).ReturnsAsync(stop);
            _mockMapper.Setup(m => m.Map<StopDto>(stop)).Returns(expectedStopDto);

            // Act
            var result = await _stopService.GetStopById(stopId);

            // Assert
            Assert.Equal(expectedStopDto.Id, result.Id);
            Assert.Equal(expectedStopDto.Name, result.Name);
            Assert.Equal(expectedStopDto.CustomerId, result.CustomerId);
        }

        [Fact]
        public async Task GetStopById_ShouldReturnNewStopDto_WhenStopDoesNotExist()
        {
            // Arrange
            var stopId = Guid.NewGuid();
            var expectedStopDto = new StopDto();

            _mockStopRepository.Setup(repo => repo.GetByIdAsync(stopId)).ReturnsAsync((Stop)null);
            _mockMapper.Setup(m => m.Map<StopDto>(It.IsAny<Stop>())).Returns(expectedStopDto);

            // Act
            var result = await _stopService.GetStopById(stopId);

            // Assert
            Assert.Equal(expectedStopDto.Id, result.Id);
            Assert.Equal(expectedStopDto.Name, result.Name);
            Assert.Equal(expectedStopDto.CustomerId, result.CustomerId);
        }

        [Fact]
        public async Task GetStopById_ShouldMapToStopDto_WhenStopIsFound()
        {
            // Arrange
            var stopId = Guid.NewGuid();
            var stop = new Stop { Id = stopId, Name = "Sample Stop", CustomerId = Guid.NewGuid() };
            var expectedStopDto = new StopDto { Id = stopId, Name = "Sample Stop", CustomerId = stop.CustomerId };

            _mockStopRepository.Setup(repo => repo.GetByIdAsync(stopId)).ReturnsAsync(stop);
            _mockMapper.Setup(m => m.Map<StopDto>(stop)).Returns(expectedStopDto);

            // Act
            var result = await _stopService.GetStopById(stopId);

            // Assert
            _mockMapper.Verify(m => m.Map<StopDto>(stop), Times.Once);
            Assert.Equal(expectedStopDto.Id, result.Id);
            Assert.Equal(expectedStopDto.Name, result.Name);
            Assert.Equal(expectedStopDto.CustomerId, result.CustomerId);
        }
    }
}

