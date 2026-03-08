using backend.Application.DTOs;
using backend.Domain.Interfaces;

namespace backend.Application.Adapters
{
    /// <summary>
    /// Adaptador para convertir entre DTOs de filtrado y criterios del dominio
    /// Mantiene la separación entre capas de Clean Architecture
    /// </summary>
    public static class DestinationFilterAdapter
    {
        /// <summary>
        /// Convierte DestinationFilterDto a IFilterCriteria del dominio
        /// </summary>
        /// <param name="dto">DTO de filtrado de la capa de aplicación</param>
        /// <returns>Criterios de filtrado del dominio</returns>
        public static IFilterCriteria ToDomainCriteria(DestinationFilterDto dto)
        {
            return new DomainFilterCriteria
            {
                SearchTerm = dto.SearchTerm,
                CountryCode = dto.CountryCode,
                DestinationTypeId = dto.DestinationTypeId,
                Page = dto.Page,
                PageSize = dto.PageSize
            };
        }
    }

    /// <summary>
    /// Implementación concreta de IFilterCriteria para el dominio
    /// </summary>
    internal class DomainFilterCriteria : IFilterCriteria
    {
        public string? SearchTerm { get; set; }
        public string? CountryCode { get; set; }
        public int? DestinationTypeId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
