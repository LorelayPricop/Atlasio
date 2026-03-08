using backend.Application.DTOs;
using backend.Domain.Entities;

namespace backend.Application.Adapters
{
    /// <summary>
    /// Mapper específico para conversiones de Destination
    /// Proporciona métodos estáticos para conversiones específicas
    /// </summary>
    public static class DestinationMapper
    {
        /// <summary>
        /// Optimiza URL de imagen agregando parámetros si es de Unsplash
        /// </summary>
        private static string? OptimizeImageUrl(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return imageUrl;

            // Si es una URL de Unsplash sin parámetros, agregar optimizaciones
            if (imageUrl.StartsWith("https://images.unsplash.com/photo-") && !imageUrl.Contains("?"))
            {
                return $"{imageUrl}?w=800&h=600&fit=crop&auto=format&q=80";
            }

            // Si ya tiene parámetros pero no todos, agregarlos
            if (imageUrl.StartsWith("https://images.unsplash.com/photo-") && !imageUrl.Contains("auto=format"))
            {
                var separator = imageUrl.Contains("?") ? "&" : "?";
                return $"{imageUrl}{separator}w=800&h=600&fit=crop&auto=format&q=80";
            }

            return imageUrl;
        }

        /// <summary>
        /// Convierte entidad Destination a DestinationDto
        /// </summary>
        public static DestinationDto ToDto(Destination entity)
        {
            return new DestinationDto
            {
                ID = entity.ID,
                Name = entity.Name,
                Description = entity.Description,
                LongDescription = entity.LongDescription,
                CountryCode = entity.CountryCode,
                DestinationTypeId = entity.DestinationTypeId,
                TypeName = entity.Type?.Name,
                LastModif = entity.LastModif,
                ImageUrl = OptimizeImageUrl(entity.ImageUrl),
                TotalBookings = entity.TotalBookings,
                AverageRating = entity.AverageRating,
                ReviewCount = entity.ReviewCount,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate
            };
        }

        /// <summary>
        /// Convierte CreateDestinationDto a entidad Destination
        /// </summary>
        public static Destination ToEntity(CreateDestinationDto dto)
        {
            return new Destination
            {
                Name = dto.Name,
                Description = dto.Description,
                LongDescription = dto.LongDescription,
                ImageUrl = dto.ImageUrl,
                CountryCode = dto.CountryCode,
                DestinationTypeId = dto.DestinationTypeId,
                LastModif = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                Status = "Active"
            };
        }

        /// <summary>
        /// Actualiza entidad existente con datos de UpdateDestinationDto
        /// </summary>
        public static void UpdateEntity(UpdateDestinationDto dto, Destination existingEntity)
        {
            existingEntity.Name = dto.Name;
            existingEntity.Description = dto.Description;
            existingEntity.LongDescription = dto.LongDescription;
            existingEntity.ImageUrl = dto.ImageUrl;
            existingEntity.CountryCode = dto.CountryCode;
            existingEntity.DestinationTypeId = dto.DestinationTypeId;
            existingEntity.LastModif = DateTime.UtcNow;
        }

        /// <summary>
        /// Convierte lista de entidades a lista de DTOs
        /// </summary>
        public static List<DestinationDto> ToDtoList(IEnumerable<Destination> entities)
        {
            return entities.Select(ToDto).ToList();
        }
    }
}
