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
	public class AddDispatchGroupsTests
	{
        private readonly DispatchGroupService _dispatchGroupService;
        private readonly Mock<IDispatchGroupRepository> _mockDispatchGroupRepository;
        private readonly Mock<IRouteRepository> _mockRouteRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<DispatchGroupService>> _mockLogger;

        public AddDispatchGroupsTests()
        {
            _mockDispatchGroupRepository = new Mock<IDispatchGroupRepository>();
            _mockRouteRepository = new Mock<IRouteRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<DispatchGroupService>>();

            _dispatchGroupService = new DispatchGroupService(_mockDispatchGroupRepository.Object, _mockRouteRepository.Object , _mockMapper.Object, _mockLogger.Object);
        }

        [Theory]
        [MemberData(nameof(GetInvalidDispatchGroupsLists))]
        public async Task AddDispatchGroups_ShouldReturnFalse_WhenInputListIsNullOrEmpty(IEnumerable<DispatchGroupDto> dispatchGroupsList)
        {
            // Act
            var result = await _dispatchGroupService.AddDispatchGroups(dispatchGroupsList);

            // Assert
            Assert.False(result);
            _mockMapper.Verify(mapper => mapper.Map<IEnumerable<DispatchGroup>>(It.IsAny<IEnumerable<DispatchGroupDto>>()), Times.Never);
            _mockDispatchGroupRepository.Verify(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<DispatchGroup>>()), Times.Never);
        }

        public static IEnumerable<object[]> GetInvalidDispatchGroupsLists()
        {
            yield return new object[] { null };
            yield return new object[] { new List<DispatchGroupDto>() };
        }

        [Fact]
        public async Task AddDispatchGroups_ShouldReturnFalse_WhenRepositoryThrowsException()
        {
            // Arrange
            var routeId = Guid.NewGuid();
            var dispatchGroups = new List<DispatchGroupDto>
            {
                new DispatchGroupDto { Name = "Dispatch Group 1", RoutesIds = new List<Guid> { routeId } }
            };

            var mappedDispatchGroups = new List<DispatchGroup>
            {
                new DispatchGroup { Id = Guid.NewGuid(), Name = "Dispatch Group 1", Routes = new List<Route>() }
            };

            _mockRouteRepository
                .Setup(repo => repo.GetByIdAsync(routeId))
                .ReturnsAsync(new Route { Id = routeId });

            _mockDispatchGroupRepository
                .Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<DispatchGroup>>()))
                .ThrowsAsync(new Exception("Repository exception"));

            // Act
            var result = await _dispatchGroupService.AddDispatchGroups(dispatchGroups);

            // Assert
            Assert.False(result);

            // Verify logging was called
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Exception while inserting DispatchGroup records")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );

            // Verify GetByIdAsync was called
            _mockRouteRepository.Verify(repo => repo.GetByIdAsync(routeId), Times.Once);
        }

        [Fact]
        public async Task AddDispatchGroups_ShouldReturnTrue_WhenDispatchGroupsAreSuccessfullyAdded()
        {
            // Arrange
            var routeId1 = Guid.NewGuid();
            var routeId2 = Guid.NewGuid();

            var dispatchGroupsList = new List<DispatchGroupDto>
            {
                new DispatchGroupDto { Name = "Group 1", RoutesIds = new List<Guid> { routeId1 } },
                new DispatchGroupDto { Name = "Group 2", RoutesIds = new List<Guid> { routeId2 } }
            };

            var route1 = new Route { Id = routeId1 };
            var route2 = new Route { Id = routeId2 };

            _mockRouteRepository
                .Setup(repo => repo.GetByIdAsync(routeId1))
                .ReturnsAsync(route1);

            _mockRouteRepository
                .Setup(repo => repo.GetByIdAsync(routeId2))
                .ReturnsAsync(route2);

            _mockDispatchGroupRepository
                .Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<DispatchGroup>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _dispatchGroupService.AddDispatchGroups(dispatchGroupsList);

            // Assert
            Assert.True(result);

            _mockRouteRepository.Verify(repo => repo.GetByIdAsync(routeId1), Times.Once);
            _mockRouteRepository.Verify(repo => repo.GetByIdAsync(routeId2), Times.Once);
            _mockDispatchGroupRepository.Verify(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<DispatchGroup>>()), Times.Once);
        }
    }
}

