namespace backend.Domain.Entities
{
    /// <summary>
    /// Entidad que representa un tipo de destino en el catálogo
    /// Reemplaza el enum DestinationType para mayor flexibilidad
    /// </summary>
    public class DestinationType
    {
        /// <summary>
        /// Identificador único
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Código único para el tipo de destino (ej: BEACH, MOUNTAIN, CITY)
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// Nombre para mostrar del tipo de destino
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Identificador de icono opcional o clase CSS para representación en UI
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// Color de fondo en formato hexadecimal (ej: #dbeafe)
        /// </summary>
        public string? ColorBackground { get; set; }

        /// <summary>
        /// Color de texto/primer plano en formato hexadecimal (ej: #0284c7)
        /// </summary>
        public string? ColorForeground { get; set; }

        /// <summary>
        /// Orden para mostrar en listas y dropdowns
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Indica si este tipo está activo para uso
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
        /// Destinos de este tipo
        /// </summary>
        public ICollection<Destination> Destinations { get; set; } = [];
    }
}
