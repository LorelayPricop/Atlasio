using Xunit;
using FluentAssertions;
using backend.Domain.Entities;
using backend.Domain.Enums;

namespace backend.Tests.Domain.Entities
{
    /// <summary>
    /// Tests unitarios para la entidad Destination
    /// Verifica solo la lógica de negocio esencial
    /// </summary>
    public class DestinationTests
    {
        [Theory]
        [InlineData(DestinationType.Beach)]
        [InlineData(DestinationType.Mountain)]
        [InlineData(DestinationType.City)]
        [InlineData(DestinationType.Cultural)]
        [InlineData(DestinationType.Adventure)]
        [InlineData(DestinationType.Relax)]
        public void Destination_ShouldAcceptAllDestinationTypes(DestinationType type)
        {
            // Arrange & Act
            var destination = new Destination
            {
                Name = "Test Destination",
                Description = "Test Description",
                CountryCode = "MEX",
                Type = type
            };

            // Assert
            destination.Type.Should().Be(type);
        }
    }
}
