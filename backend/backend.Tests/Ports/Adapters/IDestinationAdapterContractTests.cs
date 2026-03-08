using Xunit;
using FluentAssertions;
using backend.Application.DTOs;
using backend.Domain.Entities;
using backend.Domain.Interfaces;
using backend.Tests.Helpers;

namespace backend.Tests.Ports.Adapters
{
    /// <summary>
    /// Tests de contrato para IDestinationAdapter
    /// Verifica que cualquier implementación del puerto cumpla con el contrato
    /// </summary>
    public abstract class IDestinationAdapterContractTests
    {
        protected abstract IDestinationAdapter CreateAdapter();

        [Fact]
        public void ToDto_WithValidEntity_ShouldReturnCorrectDto()
        {
            // Arrange
            var adapter = CreateAdapter();
            var entity = TestDataHelper.CreateTestDestination();

            // Act
            var result = adapter.ToDto(entity);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<DestinationDto>();
            
            var dto = (DestinationDto)result;
            dto.ID.Should().Be(entity.ID);
            dto.Name.Should().Be(entity.Name);
            dto.Description.Should().Be(entity.Description);
            dto.CountryCode.Should().Be(entity.CountryCode);
            dto.DestinationTypeId.Should().Be(entity.DestinationTypeId);
            dto.LastModif.Should().Be(entity.LastModif);
        }

        [Fact]
        public void ToEntity_WithCreateDestinationDto_ShouldReturnCorrectEntity()
        {
            // Arrange
            var adapter = CreateAdapter();
            var createDto = TestDataHelper.CreateTestCreateDestinationDto();

            // Act
            var result = adapter.ToEntity(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(createDto.Name);
            result.Description.Should().Be(createDto.Description);
            result.CountryCode.Should().Be(createDto.CountryCode);
            result.DestinationTypeId.Should().Be(createDto.DestinationTypeId);
            result.LastModif.Should().BeCloseTo(DateTime.UtcNow, TestConstants.DateTimeTolerance);
        }

        [Fact]
        public void ToEntity_WithUpdateDestinationDto_ShouldReturnCorrectEntity()
        {
            // Arrange
            var adapter = CreateAdapter();
            var updateDto = TestDataHelper.CreateTestUpdateDestinationDto();

            // Act
            var result = adapter.ToEntity(updateDto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(updateDto.Name);
            result.Description.Should().Be(updateDto.Description);
            result.CountryCode.Should().Be(updateDto.CountryCode);
            result.DestinationTypeId.Should().Be(updateDto.DestinationTypeId);
            result.LastModif.Should().BeCloseTo(DateTime.UtcNow, TestConstants.DateTimeTolerance);
        }

        [Fact]
        public void ToEntity_WithUnsupportedDto_ShouldThrowArgumentException()
        {
            // Arrange
            var adapter = CreateAdapter();
            var unsupportedDto = new { Name = "Test" };

            // Act & Assert
            var action = () => adapter.ToEntity(unsupportedDto);
            action.Should().Throw<ArgumentException>()
                  .WithMessage("*Tipo de DTO no soportado*");
        }

        [Fact]
        public void UpdateEntity_WithUpdateDestinationDto_ShouldUpdateEntity()
        {
            // Arrange
            var adapter = CreateAdapter();
            var existingEntity = TestDataHelper.CreateTestDestination();
            var updateDto = TestDataHelper.CreateTestUpdateDestinationDto();
            var originalLastModif = existingEntity.LastModif;

            // Act
            adapter.UpdateEntity(updateDto, existingEntity);

            // Assert
            existingEntity.Name.Should().Be(updateDto.Name);
            existingEntity.Description.Should().Be(updateDto.Description);
            existingEntity.CountryCode.Should().Be(updateDto.CountryCode);
            existingEntity.DestinationTypeId.Should().Be(updateDto.DestinationTypeId);
            existingEntity.LastModif.Should().BeAfter(originalLastModif);
        }

        [Fact]
        public void UpdateEntity_WithUnsupportedDto_ShouldThrowArgumentException()
        {
            // Arrange
            var adapter = CreateAdapter();
            var existingEntity = TestDataHelper.CreateTestDestination();
            var unsupportedDto = new { Name = "Test" };

            // Act & Assert
            var action = () => adapter.UpdateEntity(unsupportedDto, existingEntity);
            action.Should().Throw<ArgumentException>()
                  .WithMessage("*Tipo de DTO no soportado para actualización*");
        }
    }

    /// <summary>
    /// Tests de contrato para IDestinationAdapter usando implementación concreta
    /// </summary>
    public class DestinationAdapterContractTests : IDestinationAdapterContractTests
    {
        protected override IDestinationAdapter CreateAdapter()
        {
            return new backend.Application.Adapters.DestinationAdapter();
        }
    }
}
