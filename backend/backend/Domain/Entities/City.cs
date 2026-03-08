namespace backend.Domain.Entities
{
    /// <summary>
    /// Entidad que representa una ciudad en el catálogo
    /// </summary>
    public class City
    {
        /// <summary>
        /// Identificador único
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Clave foránea a Country
        /// </summary>
        public required string CountryCode { get; set; }

        /// <summary>
        /// Nombre de la ciudad
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Indica si la ciudad está activa para la creación de destinos
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
        /// País al que pertenece esta ciudad
        /// </summary>
        public Country? Country { get; set; }

        /// <summary>
        /// Destinos en esta ciudad
        /// </summary>
        public ICollection<Destination> Destinations { get; set; } = [];
    }
}
