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
    /// Tests unitarios para CreateDestinationCommandHandler
    /// Verifica la lógica de creación de destinos
    /// </summary>
    public class CreateDestinationCommandHandlerTests
    {
        private readonly Mock<IRepositoryManager> _mockRepositoryManager;
        private readonly CreateDestinationCommandHandler _handler;

        public CreateDestinationCommandHandlerTests()
        {
            _mockRepositoryManager = new Mock<IRepositoryManager>();
            _handler = new CreateDestinationCommandHandler(_mockRepositoryManager.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCreateDestination()
        {
            // Arrange
            var createDto = TestDataHelper.CreateTestCreateDestinationDto();
            var command = new CreateDestinationCommand { CreateDestinationDto = createDto };
            
            // Setup mock para el repositorio
            var mockRepository = new Mock<backend.Domain.Interfaces.IDestinationRepository>();
            _mockRepositoryManager.Setup(r => r.Destinations).Returns(mockRepository.Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(createDto.Name);
            result.Description.Should().Be(createDto.Description);
            result.CountryCode.Should().Be(createDto.CountryCode);
            result.DestinationTypeId.Should().Be(createDto.DestinationTypeId);
            mockRepository.Verify(r => r.Add(It.IsAny<Destination>()), Times.Once);
            _mockRepositoryManager.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldSetLastModifToCurrentTime()
        {
            // Arrange
            var createDto = TestDataHelper.CreateTestCreateDestinationDto();
            var command = new CreateDestinationCommand { CreateDestinationDto = createDto };
            
            // Setup mock para el repositorio
            var mockRepository = new Mock<backend.Domain.Interfaces.IDestinationRepository>();
            _mockRepositoryManager.Setup(r => r.Destinations).Returns(mockRepository.Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.LastModif.Should().BeCloseTo(DateTime.UtcNow, TestConstants.DateTimeTolerance);
        }
    }
}
