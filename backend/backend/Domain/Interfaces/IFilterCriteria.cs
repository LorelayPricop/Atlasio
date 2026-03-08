namespace backend.Domain.Interfaces
{
    /// <summary>
    /// Interfaz genérica para criterios de filtrado
    /// Permite que el dominio no dependa de DTOs específicos de Application
    /// </summary>
    public interface IFilterCriteria
    {
        /// <summary>Término de búsqueda que se aplica a nombre, descripción y código de país</summary>
        string? SearchTerm { get; set; }
        /// <summary>Filtro por código de país específico</summary>
        string? CountryCode { get; set; }
        /// <summary>Filtro por ID de tipo de destino específico</summary>
        int? DestinationTypeId { get; set; }
        /// <summary>Número de página actual (comienza en 1)</summary>
        int Page { get; set; }
        /// <summary>Número de elementos por página (máximo 100 recomendado)</summary>
        int PageSize { get; set; }
    }
}
