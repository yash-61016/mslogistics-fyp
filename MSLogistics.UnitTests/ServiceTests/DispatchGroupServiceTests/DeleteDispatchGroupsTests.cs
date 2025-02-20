using System;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MSLogistics.Application.Repositories.IDispatchGroupRepository;
using MSLogistics.Application.Repositories.IRouteRepository;
using MSLogistics.Application.Services.DispatchGroupService;
using MSLogistics.Domain;
using Xunit;

namespace MSLogistics.UnitTests.ServiceTests.DispatchGroupServiceTests
{
	public class DeleteDispatchGroupsTests
	{
        private readonly DispatchGroupService _dispatchGroupService;
        private readonly Mock<IDispatchGroupRepository> _mockDispatchGroupRepository;
        private readonly Mock<IRouteRepository> _mockRouteRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<DispatchGroupService>> _mockLogger;

        public DeleteDispatchGroupsTests()
        {
            _mockDispatchGroupRepository = new Mock<IDispatchGroupRepository>();
            _mockRouteRepository = new Mock<IRouteRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<DispatchGroupService>>();

            _dispatchGroupService = new DispatchGroupService(_mockDispatchGroupRepository.Object, _mockRouteRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task DeleteDispatchGroups_ShouldReturnFalse_WhenIdsIsNull()
        {
            // Act
            var result = await _dispatchGroupService.DeleteDispatchGroups(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteDispatchGroups_ShouldReturnFalse_WhenIdsIsEmpty()
        {
            // Act
            var result = await _dispatchGroupService.DeleteDispatchGroups(new List<Guid>());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteDispatchGroups_ShouldLogError_WhenDispatchGroupNotFound()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };

            _mockDispatchGroupRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((DispatchGroup?)null);

            // Act
            var result = await _dispatchGroupService.DeleteDispatchGroups(ids);

            // Assert
            Assert.False(result);

            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Stop with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteDispatchGroups_ShouldReturnTrue_WhenAllDispatchGroupsDeletedSuccessfully()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var dispatchGroups = ids.Select(id => new DispatchGroup { Id = id }).ToList();

            foreach (var dispatchGroup in dispatchGroups)
            {
                _mockDispatchGroupRepository
                    .Setup(repo => repo.GetByIdAsync(dispatchGroup.Id))
                    .ReturnsAsync(dispatchGroup);
            }

            _mockDispatchGroupRepository
                .Setup(repo => repo.DeleteRangeAsync(It.IsAny<ICollection<Guid>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _dispatchGroupService.DeleteDispatchGroups(ids);

            // Assert
            Assert.True(result);
            _mockDispatchGroupRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Exactly(ids.Count));
            _mockDispatchGroupRepository.Verify(repo => repo.DeleteRangeAsync(It.IsAny<ICollection<Guid>>()), Times.Once);
        }

        [Fact]
        public async Task DeleteDispatchGroups_ShouldReturnFalse_WhenRepositoryThrowsException()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };

            _mockDispatchGroupRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new DispatchGroup { Id = ids.First() });

            _mockDispatchGroupRepository
                .Setup(repo => repo.DeleteRangeAsync(It.IsAny<ICollection<Guid>>()))
                .ThrowsAsync(new Exception("Repository exception"));

            // Act
            var result = await _dispatchGroupService.DeleteDispatchGroups(ids);

            // Assert
            Assert.False(result);

            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Exception was thrown while deleting a range of records")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
}

