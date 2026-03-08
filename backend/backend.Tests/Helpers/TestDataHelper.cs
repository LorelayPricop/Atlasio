using backend.Domain.Entities;
using backend.Domain.Enums;
using backend.Application.DTOs;

namespace backend.Tests.Helpers
{
    /// <summary>
    /// Clase helper para crear datos de prueba en los tests unitarios
    /// </summary>
    public static class TestDataHelper
    {
        /// <summary>
        /// Crea un destino de prueba individual
        /// </summary>
        public static Destination CreateTestDestination()
        {
            return new Destination
            {
                ID = 1,
                Name = "Cancún",
                Description = "Hermosa playa en el Caribe mexicano",
                CountryCode = "MEX",
                Type = DestinationType.Beach,
                LastModif = DateTime.UtcNow.AddDays(-1),
                CreatedDate = DateTime.UtcNow.AddDays(-7),
                Status = "Active"
            };
        }

        /// <summary>
        /// Crea una lista de destinos de prueba
        /// </summary>
        public static List<Destination> CreateTestDestinations()
        {
            return
            [
                new Destination
                {
                    ID = 1,
                    Name = "Cancún",
                    Description = "Hermosa playa en el Caribe mexicano",
                    CountryCode = "MEX",
                    Type = DestinationType.Beach,
                    LastModif = DateTime.UtcNow.AddDays(-3),
                    CreatedDate = DateTime.UtcNow.AddDays(-10),
                    Status = "Active"
                },
                new Destination
                {
                    ID = 2,
                    Name = "París",
                    Description = "La ciudad de la luz y el amor",
                    CountryCode = "FRA",
                    Type = DestinationType.City,
                    LastModif = DateTime.UtcNow.AddDays(-2),
                    CreatedDate = DateTime.UtcNow.AddDays(-8),
                    Status = "Active"
                },
                new Destination
                {
                    ID = 3,
                    Name = "Tokio",
                    Description = "Metrópolis moderna con tradición milenaria",
                    CountryCode = "JPN",
                    Type = DestinationType.Cultural,
                    LastModif = DateTime.UtcNow.AddDays(-1),
                    CreatedDate = DateTime.UtcNow.AddDays(-5),
                    Status = "Active"
                }
            ];
        }

        /// <summary>
        /// Crea un DTO de creación de destino de prueba
        /// </summary>
        public static CreateDestinationDto CreateTestCreateDestinationDto()
        {
            return new CreateDestinationDto
            {
                Name = "Barcelona",
                Description = "Ciudad cosmopolita con arquitectura única",
                CountryCode = "ESP",
                Type = DestinationType.Cultural
            };
        }

        /// <summary>
        /// Crea un DTO de actualización de destino de prueba
        /// </summary>
        public static UpdateDestinationDto CreateTestUpdateDestinationDto()
        {
            return new UpdateDestinationDto
            {
                Name = "Barcelona Actualizada",
                Description = "Descripción actualizada de Barcelona",
                CountryCode = "ESP",
                Type = DestinationType.City
            };
        }

        /// <summary>
        /// Crea un DTO de destino de prueba
        /// </summary>
        public static DestinationDto CreateTestDestinationDto()
        {
            return new DestinationDto
            {
                ID = 1,
                Name = "Cancún",
                Description = "Hermosa playa en el Caribe mexicano",
                CountryCode = "MEX",
                Type = DestinationType.Beach,
                LastModif = DateTime.UtcNow.AddDays(-1),
                CreatedDate = DateTime.UtcNow.AddDays(-7)
            };
        }

        /// <summary>
        /// Crea un filtro de destino de prueba
        /// </summary>
        public static DestinationFilterDto CreateTestDestinationFilter()
        {
            return new DestinationFilterDto
            {
                SearchTerm = "test",
                CountryCode = "MEX",
                Type = DestinationType.Beach,
                Page = 1,
                PageSize = 10
            };
        }
    }
}
