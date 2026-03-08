namespace backend.Application.DTOs
{
    /// <summary>
    /// DTO para catálogo de países
    /// </summary>
    public record CountryDto
    {
        /// <summary>Código de país ISO 3166-1 alpha-3</summary>
        public required string Code { get; init; }

        /// <summary>Nombre completo del país</summary>
        public required string Name { get; init; }

        /// <summary>Región geográfica</summary>
        public string? Region { get; init; }

        /// <summary>Indica si el país está activo</summary>
        public bool IsActive { get; init; }
    }

    /// <summary>
    /// DTO para catálogo de tipos de destino
    /// </summary>
    public record DestinationTypeDto
    {
        /// <summary>Identificador único</summary>
        public int Id { get; init; }

        /// <summary>Código único</summary>
        public required string Code { get; init; }

        /// <summary>Nombre para mostrar</summary>
        public required string Name { get; init; }

        /// <summary>Identificador de icono o clase CSS</summary>
        public string? Icon { get; init; }

        /// <summary>Color de fondo en formato hexadecimal (ej: #dbeafe)</summary>
        public string? ColorBackground { get; init; }

        /// <summary>Color de texto en formato hexadecimal (ej: #0284c7)</summary>
        public string? ColorForeground { get; init; }

        /// <summary>Orden de visualización</summary>
        public int DisplayOrder { get; init; }

        /// <summary>Indica si el tipo está activo</summary>
        public bool IsActive { get; init; }
    }

    /// <summary>
    /// DTO para catálogo de ciudades
    /// </summary>
    public record CityDto
    {
        /// <summary>Identificador único</summary>
        public int Id { get; init; }

        /// <summary>Código de país</summary>
        public required string CountryCode { get; init; }

        /// <summary>Nombre de la ciudad</summary>
        public required string Name { get; init; }

        /// <summary>Indica si la ciudad está activa</summary>
        public bool IsActive { get; init; }
    }
}
