namespace backend.Domain.Entities
{
    /// <summary>
    /// Entidad que representa un país en el catálogo
    /// Actúa como dato maestro para ubicaciones de destinos
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Código de país ISO 3166-1 alpha-3 (ej: USA, ESP, MEX)
        /// Clave primaria
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// Nombre completo del país
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Región geográfica (ej: Europa, Asia, Américas)
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Indica si el país está activo para la creación de destinos
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Fecha y hora de creación del registro
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha y hora de la última actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación
        /// <summary>
        /// Ciudades dentro de este país
        /// </summary>
        public ICollection<City> Cities { get; set; } = [];

        /// <summary>
        /// Destinos en este país
        /// </summary>
        public ICollection<Destination> Destinations { get; set; } = [];
    }
}
