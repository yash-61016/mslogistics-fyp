using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MSLogistics.Application.Repositories.IDispatchGroupRepository;
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
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<DispatchGroupService>> _mockLogger;

        public UpdateDispatchGroupsTests()
        {
            _mockDispatchGroupRepository = new Mock<IDispatchGroupRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<DispatchGroupService>>();

            _dispatchGroupService = new DispatchGroupService(_mockDispatchGroupRepository.Object, _mockMapper.Object, _mockLogger.Object);
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
            var dispatchGroupDto = new DispatchGroupDto { Id = Guid.NewGuid() };
            var dispatchGroup = new DispatchGroup { Id = dispatchGroupDto.Id };

            _mockDispatchGroupRepository
                .Setup(repo => repo.GetByIdAsync(dispatchGroupDto.Id))
                .ReturnsAsync(dispatchGroup);

            _mockMapper
                .Setup(mapper => mapper.Map(dispatchGroupDto, dispatchGroup))
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
        }

        [Fact]
        public async Task UpdateDispatchGroups_ShouldReturnFalse_WhenRepositoryThrowsException()
        {
            // Arrange
            var dispatchGroupDto = new DispatchGroupDto { Id = Guid.NewGuid() };
            var dispatchGroup = new DispatchGroup { Id = dispatchGroupDto.Id };

            _mockDispatchGroupRepository
                .Setup(repo => repo.GetByIdAsync(dispatchGroupDto.Id))
                .ReturnsAsync(dispatchGroup);

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
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Exception was thrown while updating a range of records")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
}

