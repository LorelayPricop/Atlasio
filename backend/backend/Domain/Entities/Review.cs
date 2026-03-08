namespace backend.Domain.Entities
{
    /// <summary>
    /// Entidad que representa una reseña de un destino
    /// </summary>
    public class Review
    {
        /// <summary>
        /// Identificador único
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Clave foránea a Destination
        /// </summary>
        public int DestinationId { get; set; }

        /// <summary>
        /// Identificador del usuario que escribió la reseña
        /// Actualmente almacenado como string, puede ser FK a tabla User en el futuro
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// Calificación de 0 a 5
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// Comentario/texto de la reseña
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Estado de la reseña (ej: Pending, Approved, Rejected)
        /// </summary>
        public required string Status { get; set; } = "Pending";

        /// <summary>
        /// Fecha y hora de creación de la reseña
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha y hora de la última actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación
        /// <summary>
        /// Destino que está siendo reseñado
        /// </summary>
        public Destination? Destination { get; set; }
    }
}
