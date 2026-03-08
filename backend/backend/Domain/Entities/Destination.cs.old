using backend.Domain.Enums;

namespace backend.Domain.Entities
{
    /// <summary>
    /// Modelo principal que representa un destino turístico
    /// </summary>
    public class Destination
    {
        /// <summary>
        /// Identificador único del destino (clave primaria)
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nombre del destino turístico
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Descripción detallada del destino
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Código ISO del país (formato de 3 caracteres: MEX, USA, ESP, etc.)
        /// </summary>
        public required string CountryCode { get; set; }

        /// <summary>
        /// Tipo de destino turístico según la categorización
        /// </summary>
        public required DestinationType Type { get; set; }

        /// <summary>
        /// Fecha y hora de la última modificación del registro
        /// Se actualiza automáticamente en cada operación de escritura
        /// </summary>
        public DateTime LastModif { get; set; }

        /// <summary>
        /// URL de la imagen principal del destino
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Total de reservas del destino
        /// </summary>
        public int TotalBookings { get; set; }

        /// <summary>
        /// Calificación promedio (escala 0-5)
        /// </summary>
        public decimal AverageRating { get; set; }

        /// <summary>
        /// Número de reseñas
        /// </summary>
        public int ReviewCount { get; set; }

        /// <summary>
        /// Estado del destino (Active, Inactive)
        /// </summary>
        public string Status { get; set; } = "Active";

        /// <summary>
        /// Nombre del usuario que creó el registro
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
