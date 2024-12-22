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
	public class AddDispatchGroupsTests
	{
        private readonly DispatchGroupService _dispatchGroupService;
        private readonly Mock<IDispatchGroupRepository> _mockDispatchGroupRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<DispatchGroupService>> _mockLogger;

        public AddDispatchGroupsTests()
        {
            _mockDispatchGroupRepository = new Mock<IDispatchGroupRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<DispatchGroupService>>();

            _dispatchGroupService = new DispatchGroupService(_mockDispatchGroupRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AddDispatchGroups_ShouldReturnTrue_WhenDispatchGroupsAreSuccessfullyAdded()
        {
            // Arrange
            var dispatchGroupsList = new List<DispatchGroupDto>
            {
                new DispatchGroupDto { Name = "Group 1", Routes = new List<RouteDto>() },
                new DispatchGroupDto { Name = "Group 2", Routes = new List<RouteDto>() }
            };

            var dispatchGroupEntities = dispatchGroupsList.Select(dto => new DispatchGroup { Name = dto.Name }).ToList();

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<DispatchGroup>>(dispatchGroupsList))
                .Returns(dispatchGroupEntities);

            _mockDispatchGroupRepository.Setup(repo => repo.AddRangeAsync(dispatchGroupEntities))
                .ReturnsAsync(true);

            // Act
            var result = await _dispatchGroupService.AddDispatchGroups(dispatchGroupsList);

            // Assert
            Assert.True(result);
            _mockMapper.Verify(mapper => mapper.Map<IEnumerable<DispatchGroup>>(dispatchGroupsList), Times.Once);
            _mockDispatchGroupRepository.Verify(repo => repo.AddRangeAsync(dispatchGroupEntities), Times.Once);
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
            var dispatchGroups = new List<DispatchGroupDto>
            {
                new DispatchGroupDto { Name = "Dispatch Group 1" }
            };

            var mappedDispatchGroups = new List<DispatchGroup>
            {
                new DispatchGroup { Name = "Dispatch Group 1" }
            };

            _mockMapper
                .Setup(mapper => mapper.Map<IEnumerable<DispatchGroup>>(dispatchGroups))
                .Returns(mappedDispatchGroups);

            _mockDispatchGroupRepository
                .Setup(repo => repo.AddRangeAsync(mappedDispatchGroups))
                .ThrowsAsync(new Exception("Repository exception"));

            // Act
            var result = await _dispatchGroupService.AddDispatchGroups(dispatchGroups);

            // Assert
            Assert.False(result);
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Exception was thrown while inserting a range of records")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
}

