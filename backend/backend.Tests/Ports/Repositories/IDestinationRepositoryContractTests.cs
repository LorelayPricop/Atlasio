using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using backend.Infrastructure.Data;
using backend.Infrastructure.Repositories;
using backend.Domain.Entities;
using backend.Domain.Interfaces;
using backend.Tests.Helpers;

namespace backend.Tests.Ports.Repositories
{
    /// <summary>
    /// Tests de contrato para IDestinationRepository
    /// Verifica que cualquier implementación del puerto cumpla con el contrato
    /// </summary>
    public abstract class IDestinationRepositoryContractTests
    {
        protected abstract IDestinationRepository CreateRepository(ApplicationDbContext context);

        [Fact]
        public async Task GetDestinationsWithFiltersAsync_WithNoFilters_ShouldReturnAllDestinations()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var repository = CreateRepository(context);
            
            var destinations = TestDataHelper.CreateTestDestinations();
            context.Destinations.AddRange(destinations);
            await context.SaveChangesAsync();

            var filter = new TestFilterCriteria
            {
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await repository.GetDestinationsWithFiltersAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(destinations.Count);
            result.TotalCount.Should().Be(destinations.Count);
        }

        [Fact]
        public async Task GetDestinationsWithFiltersAsync_WithSearchTerm_ShouldFilterByName()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var repository = CreateRepository(context);
            
            var destinations = TestDataHelper.CreateTestDestinations();
            context.Destinations.AddRange(destinations);
            await context.SaveChangesAsync();

            var filter = new TestFilterCriteria
            {
                SearchTerm = "Cancún",
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await repository.GetDestinationsWithFiltersAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().OnlyContain(d => d.Name.Contains("Cancún", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetUniqueCountryCodesAsync_ShouldReturnDistinctCountryCodes()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var repository = CreateRepository(context);
            
            var destinations = TestDataHelper.CreateTestDestinations();
            context.Destinations.AddRange(destinations);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetUniqueCountryCodesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(destinations.Select(d => d.CountryCode).Distinct().Count());
            result.Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public async Task GetDestinationsByTypeAsync_ShouldReturnDestinationsOfSpecificType()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var repository = CreateRepository(context);

            var destinations = TestDataHelper.CreateTestDestinations();
            context.Destinations.AddRange(destinations);
            await context.SaveChangesAsync();

            // Act - Usar GetDestinationsWithFiltersAsync en lugar del método obsoleto
            var filter = new TestFilterCriteria 
            { 
                DestinationTypeId = TestDataHelper.BeachTypeId,
                Page = 1,
                PageSize = 100
            };
            var result = await repository.GetDestinationsWithFiltersAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().OnlyContain(d => d.DestinationTypeId == TestDataHelper.BeachTypeId);
        }

        [Fact]
        public async Task SearchDestinationsAsync_ShouldReturnMatchingDestinations()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var repository = CreateRepository(context);
            
            var destinations = TestDataHelper.CreateTestDestinations();
            context.Destinations.AddRange(destinations);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.SearchDestinationsAsync("París");

            // Assert
            result.Should().NotBeNull();
            result.Should().OnlyContain(d => d.Name.Contains("París", StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// Tests de contrato para IDestinationRepository usando implementación concreta
    /// </summary>
    public class DestinationRepositoryContractTests : IDestinationRepositoryContractTests
    {
        protected override IDestinationRepository CreateRepository(ApplicationDbContext context)
        {
            return new DestinationRepository(context);
        }
    }

    /// <summary>
    /// Implementación de prueba de IFilterCriteria para los tests
    /// </summary>
    internal class TestFilterCriteria : IFilterCriteria
    {
        public string? SearchTerm { get; set; }
        public string? CountryCode { get; set; }
        public int? DestinationTypeId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
