using Xunit;
using FluentAssertions;
using backend.Domain.Entities;
using backend.Tests.Helpers;

namespace backend.Tests.Domain.Entities
{
    /// <summary>
    /// Tests unitarios para la entidad Destination
    /// Verifica solo la lógica de negocio esencial
    /// </summary>
    public class DestinationTests
    {
        [Theory]
        [InlineData(TestDataHelper.BeachTypeId)]
        [InlineData(TestDataHelper.MountainTypeId)]
        [InlineData(TestDataHelper.CityTypeId)]
        [InlineData(TestDataHelper.CulturalTypeId)]
        [InlineData(TestDataHelper.AdventureTypeId)]
        [InlineData(TestDataHelper.RelaxTypeId)]
        public void Destination_ShouldAcceptAllDestinationTypes(int typeId)
        {
            // Arrange & Act
            var destination = new Destination
            {
                Name = "Test Destination",
                Description = "Test Description",
                CountryCode = "MEX",
                DestinationTypeId = typeId
            };

            // Assert
            destination.DestinationTypeId.Should().Be(typeId);
        }
    }
}
