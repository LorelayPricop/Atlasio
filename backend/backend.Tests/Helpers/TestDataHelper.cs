using backend.Domain.Entities;
using backend.Application.DTOs;

namespace backend.Tests.Helpers
{
    /// <summary>
    /// Clase helper para crear datos de prueba en los tests unitarios
    /// </summary>
    public static class TestDataHelper
    {
        // IDs de tipos de destino para tests (deben coincidir con el seed)
        public const int BeachTypeId = 1;
        public const int MountainTypeId = 2;
        public const int CityTypeId = 3;
        public const int CulturalTypeId = 4;
        public const int AdventureTypeId = 5;
        public const int RelaxTypeId = 6;

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
                DestinationTypeId = BeachTypeId,
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
                    DestinationTypeId = BeachTypeId,
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
                    DestinationTypeId = CityTypeId,
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
                    DestinationTypeId = CulturalTypeId,
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
                LongDescription = "Barcelona es una ciudad vibrante con la arquitectura de Gaudí, playas mediterráneas y una rica cultura catalana.",
                CountryCode = "ESP",
                DestinationTypeId = CulturalTypeId
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
                LongDescription = "Descripción larga actualizada con información completa sobre Barcelona.",
                CountryCode = "ESP",
                DestinationTypeId = CityTypeId
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
                DestinationTypeId = BeachTypeId,
                TypeName = "Beach",
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
                DestinationTypeId = BeachTypeId,
                Page = 1,
                PageSize = 10
            };
        }
    }
}
