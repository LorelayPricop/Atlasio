using backend.Application.DTOs;
using backend.Domain.Entities;
using backend.Domain.Interfaces;

namespace backend.Application.Adapters
{
    /// <summary>
    /// Adaptador para conversión entre entidades Destination y DTOs
    /// Implementa conversión manual sin dependencias externas
    /// </summary>
    public class DestinationAdapter : IDestinationAdapter
    {
        public object ToDto(Destination entity)
        {
            return new DestinationDto
            {
                ID = entity.ID,
                Name = entity.Name,
                Description = entity.Description,
                CountryCode = entity.CountryCode,
                Type = entity.Type,
                LastModif = entity.LastModif,
                ImageUrl = entity.ImageUrl,
                TotalBookings = entity.TotalBookings,
                AverageRating = entity.AverageRating,
                ReviewCount = entity.ReviewCount,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate
            };
        }

        public Destination ToEntity(object dto)
        {
            if (dto is CreateDestinationDto createDto)
            {
                return new Destination
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    CountryCode = createDto.CountryCode,
                    Type = createDto.Type,
                    LastModif = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    Status = "Active"
                };
            }

            if (dto is UpdateDestinationDto updateDto)
            {
                return new Destination
                {
                    Name = updateDto.Name,
                    Description = updateDto.Description,
                    CountryCode = updateDto.CountryCode,
                    Type = updateDto.Type,
                    LastModif = DateTime.UtcNow
                };
            }

            throw new ArgumentException($"Tipo de DTO no soportado: {dto.GetType().Name}", nameof(dto));
        }

        public void UpdateEntity(object dto, Destination existingEntity)
        {
            if (dto is UpdateDestinationDto updateDto)
            {
                existingEntity.Name = updateDto.Name;
                existingEntity.Description = updateDto.Description;
                existingEntity.CountryCode = updateDto.CountryCode;
                existingEntity.Type = updateDto.Type;
                existingEntity.LastModif = DateTime.UtcNow;
                return;
            }

            throw new ArgumentException($"Tipo de DTO no soportado para actualización: {dto.GetType().Name}", nameof(dto));
        }
    }
}
