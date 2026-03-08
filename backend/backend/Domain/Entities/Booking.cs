namespace backend.Domain.Entities
{
    /// <summary>
    /// Entidad que representa una reserva de un destino
    /// </summary>
    public class Booking
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
        /// Identificador del usuario que realizó la reserva
        /// Actualmente almacenado como string, puede ser FK a tabla User en el futuro
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// Fecha de inicio del viaje
        /// </summary>
        public DateTime TravelStart { get; set; }

        /// <summary>
        /// Fecha de fin del viaje
        /// </summary>
        public DateTime TravelEnd { get; set; }

        /// <summary>
        /// Estado actual de la reserva (ej: Pending, Confirmed, Cancelled, Completed)
        /// </summary>
        public required string BookingStatus { get; set; } = "Pending";

        /// <summary>
        /// Monto de la reserva
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Código de moneda (ej: USD, EUR, MXN)
        /// </summary>
        public required string Currency { get; set; } = "USD";

        /// <summary>
        /// Fecha y hora de creación de la reserva
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha y hora de la última actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación
        /// <summary>
        /// Destino que está siendo reservado
        /// </summary>
        public Destination? Destination { get; set; }
    }
}
