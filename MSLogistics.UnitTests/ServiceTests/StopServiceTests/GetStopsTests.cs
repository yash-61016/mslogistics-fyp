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
	public class GetStopsTests
	{
        private readonly StopService _stopService;
        private readonly Mock<IStopRepository> _mockStopRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<StopService>> _mockLogger;

        public GetStopsTests()
        {
            _mockStopRepository = new Mock<IStopRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<StopService>>();

            _stopService = new StopService(_mockStopRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetStops_ShouldReturnMappedStopDtos_WhenStopsExist()
        {
            // Arrange
            var stopEntities = new List<Stop>
            {
                new Stop { Id = Guid.NewGuid(), Name = "Stop1", CustomerId = Guid.NewGuid() },
                new Stop { Id = Guid.NewGuid(), Name = "Stop2", CustomerId = Guid.NewGuid() }
            };

            var stopDtos = new List<StopDto>
            {
                new StopDto { Id = stopEntities[0].Id, Name = "Stop1", CustomerId = stopEntities[0].CustomerId },
                new StopDto { Id = stopEntities[1].Id, Name = "Stop2", CustomerId = stopEntities[1].CustomerId }
            };

            _mockStopRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(stopEntities);
            _mockMapper.Setup(m => m.Map<IEnumerable<StopDto>>(stopEntities)).Returns(stopDtos);

            // Act
            var result = await _stopService.GetStops();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(stopDtos.Count, result.Count());
            _mockStopRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<StopDto>>(stopEntities), Times.Once);
        }

        [Fact]
        public async Task GetStops_ShouldReturnEmptyList_WhenNoStopsExist()
        {
            // Arrange
            _mockStopRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Stop>());
            _mockMapper.Setup(m => m.Map<IEnumerable<StopDto>>(It.IsAny<IEnumerable<Stop>>())).Returns(new List<StopDto>());

            // Act
            var result = await _stopService.GetStops();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockStopRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<StopDto>>(It.IsAny<IEnumerable<Stop>>()), Times.Once);
        }

        [Fact]
        public async Task GetStops_ShouldReturnEmptyList_WhenStopRepositoryReturnsNull()
        {
            // Arrange
            _mockStopRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync((IEnumerable<Stop>)null);
            _mockMapper.Setup(m => m.Map<IEnumerable<StopDto>>(It.IsAny<IEnumerable<Stop>>())).Returns(new List<StopDto>());

            // Act
            var result = await _stopService.GetStops();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockStopRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<StopDto>>(It.IsAny<IEnumerable<Stop>>()), Times.Once);
        }
    }
}

