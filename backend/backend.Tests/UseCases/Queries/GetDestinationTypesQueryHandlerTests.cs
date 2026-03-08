using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using backend.Application.Queries;
using backend.Application.DTOs;
using backend.Infrastructure.Data;

namespace backend.Tests.Application.Queries
{
    /// <summary>
    /// Tests unitarios para GetDestinationTypesQueryHandler
    /// Verifica la lógica de consulta de tipos de destino
    /// </summary>
    public class GetDestinationTypesQueryHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GetDestinationTypesQueryHandler _handler;

        public GetDestinationTypesQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
            _context = new ApplicationDbContext(options);
            _handler = new GetDestinationTypesQueryHandler(_context);

            // Seed tipos de destino
            SeedDestinationTypes();
        }

        private void SeedDestinationTypes()
        {
            var types = new[]
            {
                new backend.Domain.Entities.DestinationType { Code = "BEACH", Name = "Beach", Icon = "beach_access", DisplayOrder = 1 },
                new backend.Domain.Entities.DestinationType { Code = "MOUNTAIN", Name = "Mountain", Icon = "terrain", DisplayOrder = 2 },
                new backend.Domain.Entities.DestinationType { Code = "CITY", Name = "City", Icon = "location_city", DisplayOrder = 3 },
                new backend.Domain.Entities.DestinationType { Code = "CULTURAL", Name = "Cultural", Icon = "museum", DisplayOrder = 4 },
                new backend.Domain.Entities.DestinationType { Code = "ADVENTURE", Name = "Adventure", Icon = "hiking", DisplayOrder = 5 },
                new backend.Domain.Entities.DestinationType { Code = "RELAX", Name = "Relax", Icon = "spa", DisplayOrder = 6 }
            };
            _context.DestinationTypes.AddRange(types);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Handle_ShouldReturnListOfDestinationTypes()
        {
            // Arrange
            var query = new GetDestinationTypesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(6);
            result.Should().AllBeOfType<DestinationTypeDto>();
        }

        [Fact]
        public async Task Handle_ShouldReturnAllExpectedTypes()
        {
            // Arrange
            var query = new GetDestinationTypesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().ContainSingle(t => t.Code == "BEACH");
            result.Should().ContainSingle(t => t.Code == "MOUNTAIN");
            result.Should().ContainSingle(t => t.Code == "CITY");
            result.Should().ContainSingle(t => t.Code == "CULTURAL");
            result.Should().ContainSingle(t => t.Code == "ADVENTURE");
            result.Should().ContainSingle(t => t.Code == "RELAX");
        }

        [Fact]
        public async Task Handle_ShouldReturnConsistentResults()
        {
            // Arrange
            var query = new GetDestinationTypesQuery();

            // Act
            var result1 = await _handler.Handle(query, CancellationToken.None);
            var result2 = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result1.Should().BeEquivalentTo(result2);
            result1.Should().HaveCount(6);
        }

        [Fact]
        public async Task Handle_ShouldReturnTypesOrderedByDisplayOrder()
        {
            // Arrange
            var query = new GetDestinationTypesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeInAscendingOrder(t => t.DisplayOrder);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
