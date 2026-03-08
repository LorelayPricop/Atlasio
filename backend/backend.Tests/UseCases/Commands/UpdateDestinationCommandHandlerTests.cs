using Xunit;
using FluentAssertions;
using Moq;
using backend.Application.Commands;
using backend.Application.DTOs;
using backend.Domain.Entities;
using backend.Domain.Interfaces;
using backend.Tests.Helpers;

namespace backend.Tests.Application.Commands
{
    /// <summary>
    /// Tests unitarios para UpdateDestinationCommandHandler
    /// Verifica la lógica de actualización de destinos
    /// </summary>
    public class UpdateDestinationCommandHandlerTests
    {
        private readonly Mock<IRepositoryManager> _mockRepositoryManager;
        private readonly UpdateDestinationCommandHandler _handler;

        public UpdateDestinationCommandHandlerTests()
        {
            _mockRepositoryManager = new Mock<IRepositoryManager>();
            _handler = new UpdateDestinationCommandHandler(_mockRepositoryManager.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldUpdateDestination()
        {
            // Arrange
            var updateDto = TestDataHelper.CreateTestUpdateDestinationDto();
            var command = new UpdateDestinationCommand { Id = 1, UpdateDestinationDto = updateDto };
            var existingDestination = TestDataHelper.CreateTestDestination();
            var expectedDto = TestDataHelper.CreateTestDestinationDto();

            _mockRepositoryManager.Setup(r => r.Destinations.GetByIdAsync(1))
                          .ReturnsAsync(existingDestination);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(updateDto.Name);
            result.Description.Should().Be(updateDto.Description);
            result.CountryCode.Should().Be(updateDto.CountryCode);
            result.DestinationTypeId.Should().Be(updateDto.DestinationTypeId);
            _mockRepositoryManager.Verify(r => r.Destinations.Update(existingDestination), Times.Once);
            _mockRepositoryManager.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentDestination_ShouldReturnNull()
        {
            // Arrange
            var updateDto = TestDataHelper.CreateTestUpdateDestinationDto();
            var command = new UpdateDestinationCommand { Id = TestConstants.NonExistentDestinationId, UpdateDestinationDto = updateDto };

            _mockRepositoryManager.Setup(r => r.Destinations.GetByIdAsync(TestConstants.NonExistentDestinationId))
                          .ReturnsAsync((Destination?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeNull();
            _mockRepositoryManager.Verify(r => r.Destinations.Update(It.IsAny<Destination>()), Times.Never);
            _mockRepositoryManager.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldSetLastModifToCurrentTime()
        {
            // Arrange
            var updateDto = TestDataHelper.CreateTestUpdateDestinationDto();
            var command = new UpdateDestinationCommand { Id = 1, UpdateDestinationDto = updateDto };
            var existingDestination = TestDataHelper.CreateTestDestination();
            var expectedDto = TestDataHelper.CreateTestDestinationDto();

            _mockRepositoryManager.Setup(r => r.Destinations.GetByIdAsync(1))
                          .ReturnsAsync(existingDestination);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            existingDestination.LastModif.Should().BeCloseTo(DateTime.UtcNow, TestConstants.DateTimeTolerance);
        }
    }
}
