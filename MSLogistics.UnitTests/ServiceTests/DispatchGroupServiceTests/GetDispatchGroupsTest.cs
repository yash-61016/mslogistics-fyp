using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MSLogistics.Application.Repositories.IDispatchGroupRepository;
using MSLogistics.Application.Services.DispatchGroupService;
using MSLogistics.Application.ValueObjects.DTOs.DispatchGroups;
using MSLogistics.Application.ValueObjects.DTOs.Route;
using MSLogistics.Domain;
using Xunit;

namespace MSLogistics.UnitTests.ServiceTests.DispatchGroupServiceTests
{
	public class GetDispatchGroupsTest
	{
        private readonly DispatchGroupService _dispatchGroupService;
        private readonly Mock<IDispatchGroupRepository> _mockDispatchGroupRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<DispatchGroupService>> _mockLogger;

        public GetDispatchGroupsTest()
        {
            _mockDispatchGroupRepository = new Mock<IDispatchGroupRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<DispatchGroupService>>();

            _dispatchGroupService = new DispatchGroupService(_mockDispatchGroupRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetDispatchGroups_ShouldReturnMappedDispatchGroupDtos()
        {
            // Arrange
            var dispatchGroups = new List<DispatchGroup>
            {
                new DispatchGroup
                {
                    Id = Guid.NewGuid(),
                    Name = "Group 1",
                    Routes = new List<Route> { new Route { Id = Guid.NewGuid(), Name = "Route 1" } }
                },
                new DispatchGroup
                {
                    Id = Guid.NewGuid(),
                    Name = "Group 2",
                    Routes = new List<Route> { new Route { Id = Guid.NewGuid(), Name = "Route 2" } }
                }
            };

            var dispatchGroupDtos = new List<DispatchGroupDto>
            {
                new DispatchGroupDto
                {
                    Id = dispatchGroups[0].Id,
                    Name = dispatchGroups[0].Name,
                    Routes = new List<RouteDto> { new RouteDto { Id = dispatchGroups[0].Routes.First().Id, Name = dispatchGroups[0].Routes.First().Name } }
                },
                new DispatchGroupDto
                {
                    Id = dispatchGroups[1].Id,
                    Name = dispatchGroups[1].Name,
                    Routes = new List<RouteDto> { new RouteDto { Id = dispatchGroups[1].Routes.First().Id, Name = dispatchGroups[1].Routes.First().Name } }
                }
            };

            _mockDispatchGroupRepository.Setup(repo => repo.GetAllWithIncludesAsync(group => group.Routes))
                .ReturnsAsync(dispatchGroups);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<DispatchGroupDto>>(dispatchGroups))
                .Returns(dispatchGroupDtos);

            // Act
            var result = await _dispatchGroupService.GetDispatchGroups();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(dispatchGroupDtos, result);
        }

        [Fact]
        public async Task GetDispatchGroups_ShouldReturnEmptyList_WhenNoDispatchGroupsExist()
        {
            // Arrange
            _mockDispatchGroupRepository.Setup(repo => repo.GetAllWithIncludesAsync(group => group.Routes))
                .ReturnsAsync(new List<DispatchGroup>());

            // Act
            var result = await _dispatchGroupService.GetDispatchGroups();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDispatchGroups_ShouldReturnEmptyList_WhenRepositoryReturnsNull()
        {
            // Arrange
            _mockDispatchGroupRepository.Setup(repo => repo.GetAllWithIncludesAsync(group => group.Routes))
                .ReturnsAsync((IEnumerable<DispatchGroup>)null);

            // Act
            var result = await _dispatchGroupService.GetDispatchGroups();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDispatchGroups_ShouldReturnEmptyList_WhenMappingReturnsNull()
        {
            // Arrange
            var dispatchGroups = new List<DispatchGroup>
            {
                new DispatchGroup { Id = Guid.NewGuid(), Name = "Group 1" }
            };

            _mockDispatchGroupRepository.Setup(repo => repo.GetAllWithIncludesAsync(group => group.Routes))
                .ReturnsAsync(dispatchGroups);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<DispatchGroupDto>>(dispatchGroups))
                .Returns((IEnumerable<DispatchGroupDto>)null);

            // Act
            var result = await _dispatchGroupService.GetDispatchGroups();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}

