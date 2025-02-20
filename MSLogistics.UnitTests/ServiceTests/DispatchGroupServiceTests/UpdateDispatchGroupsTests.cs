using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MSLogistics.Application.Repositories.IDispatchGroupRepository;
using MSLogistics.Application.Repositories.IRouteRepository;
using MSLogistics.Application.Services.DispatchGroupService;
using MSLogistics.Application.ValueObjects.DTOs.DispatchGroups;
using MSLogistics.Domain;
using Xunit;

namespace MSLogistics.UnitTests.ServiceTests.DispatchGroupServiceTests
{
	public class UpdateDispatchGroupsTests
	{
        private readonly DispatchGroupService _dispatchGroupService;
        private readonly Mock<IDispatchGroupRepository> _mockDispatchGroupRepository;
        private readonly Mock<IRouteRepository> _mockRouteRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<DispatchGroupService>> _mockLogger;

        public UpdateDispatchGroupsTests()
        {
            _mockDispatchGroupRepository = new Mock<IDispatchGroupRepository>();
            _mockRouteRepository = new Mock<IRouteRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<DispatchGroupService>>();

            _dispatchGroupService = new DispatchGroupService(_mockDispatchGroupRepository.Object, _mockRouteRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task UpdateDispatchGroups_ShouldReturnFalse_WhenDispatchGroupsListIsNull()
        {
            // Act
            var result = await _dispatchGroupService.UpdateDispatchGroups(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateDispatchGroups_ShouldReturnFalse_WhenDispatchGroupsListIsEmpty()
        {
            // Act
            var result = await _dispatchGroupService.UpdateDispatchGroups(new List<DispatchGroupDto>());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateDispatchGroups_ShouldLogError_WhenDispatchGroupNotFound()
        {
            // Arrange
            var dispatchGroupDto = new DispatchGroupDto { Id = Guid.NewGuid() };

            _mockDispatchGroupRepository
                .Setup(repo => repo.GetByIdAsync(dispatchGroupDto.Id))
                .ReturnsAsync((DispatchGroup?)null);

            // Act
            var result = await _dispatchGroupService.UpdateDispatchGroups(new List<DispatchGroupDto> { dispatchGroupDto });

            // Assert
            Assert.False(result);

            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Dispatch group with ID {dispatchGroupDto.Id} not found for updating.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateDispatchGroups_ShouldReturnFalse_WhenNoValidDispatchGroupsToUpdate()
        {
            // Arrange
            var dispatchGroupDto = new DispatchGroupDto { Id = Guid.NewGuid() };

            _mockDispatchGroupRepository
                .Setup(repo => repo.GetByIdAsync(dispatchGroupDto.Id))
                .ReturnsAsync((DispatchGroup?)null);

            // Act
            var result = await _dispatchGroupService.UpdateDispatchGroups(new List<DispatchGroupDto> { dispatchGroupDto });

            // Assert
            Assert.False(result);

            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No valid dispatch group found to update.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateDispatchGroups_ShouldReturnTrue_WhenDispatchGroupsUpdatedSuccessfully()
        {
            // Arrange
            var routeId = Guid.NewGuid();
            var dispatchGroupDto = new DispatchGroupDto { Id = Guid.NewGuid(), RoutesIds = new List<Guid> { routeId } };
            var existingRoute = new Route { Id = routeId };
            var existingDispatchGroup = new DispatchGroup { Id = dispatchGroupDto.Id, Routes = new List<Route>() };

            _mockDispatchGroupRepository
                .Setup(repo => repo.GetDispatchGroupByIdWithIncludesAsync(dispatchGroupDto.Id, It.IsAny<Expression<Func<DispatchGroup, object>>>()))
                .ReturnsAsync(existingDispatchGroup);

            _mockRouteRepository
                .Setup(repo => repo.GetByIdAsync(routeId))
                .ReturnsAsync(existingRoute);

            _mockMapper
                .Setup(mapper => mapper.Map(dispatchGroupDto, existingDispatchGroup))
                .Verifiable();

            _mockDispatchGroupRepository
                .Setup(repo => repo.UpdateRangeAsync(It.IsAny<List<DispatchGroup>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _dispatchGroupService.UpdateDispatchGroups(new List<DispatchGroupDto> { dispatchGroupDto });

            // Assert
            Assert.True(result);
            _mockMapper.Verify();
            _mockDispatchGroupRepository.Verify(repo => repo.UpdateRangeAsync(It.IsAny<List<DispatchGroup>>()), Times.Once);
            _mockRouteRepository.Verify(repo => repo.GetByIdAsync(routeId), Times.Once);
        }

        [Fact]
        public async Task UpdateDispatchGroups_ShouldReturnFalse_WhenRepositoryThrowsException()
        {
            // Arrange
            var dispatchGroupDto = new DispatchGroupDto { Id = Guid.NewGuid(), RoutesIds = new List<Guid>() };
            var existingDispatchGroup = new DispatchGroup { Id = dispatchGroupDto.Id, Routes = new List<Route>() };

            _mockDispatchGroupRepository
                .Setup(repo => repo.GetDispatchGroupByIdWithIncludesAsync(dispatchGroupDto.Id, It.IsAny<Expression<Func<DispatchGroup, object>>>()))
                .ReturnsAsync(existingDispatchGroup);

            _mockDispatchGroupRepository
                .Setup(repo => repo.UpdateRangeAsync(It.IsAny<List<DispatchGroup>>()))
                .ThrowsAsync(new Exception("Repository exception"));

            // Act
            var result = await _dispatchGroupService.UpdateDispatchGroups(new List<DispatchGroupDto> { dispatchGroupDto });

            // Assert
            Assert.False(result);

            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Exception while updating DispatchGroups")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

    }
}

