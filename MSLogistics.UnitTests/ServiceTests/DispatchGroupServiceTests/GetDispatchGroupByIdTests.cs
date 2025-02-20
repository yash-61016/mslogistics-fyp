using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MSLogistics.Application.Repositories.IDispatchGroupRepository;
using MSLogistics.Application.Repositories.IRouteRepository;
using MSLogistics.Application.Services.DispatchGroupService;
using MSLogistics.Application.ValueObjects.DTOs.DispatchGroups;
using MSLogistics.Application.ValueObjects.DTOs.Route;
using MSLogistics.Domain;
using Xunit;

namespace MSLogistics.UnitTests.ServiceTests.DispatchGroupServiceTests
{
	public class GetDispatchGroupByIdTests
	{
        private readonly DispatchGroupService _dispatchGroupService;
        private readonly Mock<IDispatchGroupRepository> _mockDispatchGroupRepository;
        private readonly Mock<IRouteRepository> _mockRouteRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<DispatchGroupService>> _mockLogger;

        public GetDispatchGroupByIdTests()
        {
            _mockDispatchGroupRepository = new Mock<IDispatchGroupRepository>();
            _mockRouteRepository = new Mock<IRouteRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<DispatchGroupService>>();

            _dispatchGroupService = new DispatchGroupService(_mockDispatchGroupRepository.Object, _mockRouteRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetDispatchGroupById_ShouldReturnMappedDispatchGroupDto_WhenDispatchGroupExists()
        {
            // Arrange
            var dispatchGroupId = Guid.NewGuid();
            var dispatchGroup = new DispatchGroup
            {
                Id = dispatchGroupId,
                Name = "Group 1",
                Routes = new List<Route> { new Route { Id = Guid.NewGuid(), Name = "Route 1" } }
            };

            var dispatchGroupDto = new DispatchGroupDto
            {
                Id = dispatchGroupId,
                Name = dispatchGroup.Name,
                RoutesIds = new List<Guid> { dispatchGroup.Routes.First().Id }
            };

            _mockDispatchGroupRepository.Setup(repo => repo.GetDispatchGroupByIdWithIncludesAsync(dispatchGroupId, group => group.Routes))
                .ReturnsAsync(dispatchGroup);

            _mockMapper.Setup(mapper => mapper.Map<DispatchGroupDto>(dispatchGroup))
                .Returns(dispatchGroupDto);

            // Act
            var result = await _dispatchGroupService.GetDispatchGroupById(dispatchGroupId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dispatchGroupDto.Id, result.Id);
            Assert.Equal(dispatchGroupDto.Name, result.Name);
            Assert.Equal(dispatchGroupDto.RoutesIds.Count, result.RoutesIds.Count);
        }

        [Fact]
        public async Task GetDispatchGroupById_ShouldReturnEmptyDto_WhenDispatchGroupDoesNotExist()
        {
            // Arrange
            var dispatchGroupId = Guid.NewGuid();

            _mockDispatchGroupRepository.Setup(repo => repo.GetDispatchGroupByIdWithIncludesAsync(dispatchGroupId, group => group.Routes))
                .ReturnsAsync((DispatchGroup)null);

            _mockMapper.Setup(mapper => mapper.Map<DispatchGroupDto>(It.IsAny<DispatchGroup>()))
                .Returns((DispatchGroupDto)null);

            // Act
            var result = await _dispatchGroupService.GetDispatchGroupById(dispatchGroupId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Guid.Empty, result.Id);
            Assert.Null(result.Name);
            Assert.Empty(result.RoutesIds);
        }

        [Fact]
        public async Task GetDispatchGroupById_ShouldReturnEmptyDto_WhenMappingReturnsNull()
        {
            // Arrange
            var dispatchGroupId = Guid.NewGuid();
            var dispatchGroup = new DispatchGroup
            {
                Id = dispatchGroupId,
                Name = "Group 1"
            };

            _mockDispatchGroupRepository.Setup(repo => repo.GetDispatchGroupByIdWithIncludesAsync(dispatchGroupId, group => group.Routes))
                .ReturnsAsync(dispatchGroup);

            _mockMapper.Setup(mapper => mapper.Map<DispatchGroupDto>(dispatchGroup))
                .Returns((DispatchGroupDto)null);

            // Act
            var result = await _dispatchGroupService.GetDispatchGroupById(dispatchGroupId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Guid.Empty, result.Id);
            Assert.Null(result.Name);
            Assert.Empty(result.RoutesIds);
        }

        [Fact]
        public async Task GetDispatchGroupById_ShouldReturnEmptyDto_WhenRepoReturnsNull()
        {
            // Arrange
            var dispatchGroupId = Guid.NewGuid();

            _mockDispatchGroupRepository.Setup(repo => repo.GetDispatchGroupByIdWithIncludesAsync(dispatchGroupId, group => group.Routes))
                .ReturnsAsync((DispatchGroup)null); // Simulate the repository returning null.

            // Act
            var result = await _dispatchGroupService.GetDispatchGroupById(dispatchGroupId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Guid.Empty, result.Id);
            Assert.Null(result.Name);
            Assert.Empty(result.RoutesIds);
        }
    }
}

