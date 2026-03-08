namespace backend.Domain.Entities
{
    /// <summary>
    /// Entidad que representa estadísticas agregadas de un destino
    /// Desnormalizada para rendimiento - actualizada por job en segundo plano o trigger
    /// </summary>
    public class DestinationStats
    {
        /// <summary>
        /// Clave foránea a Destination (también clave primaria)
        /// </summary>
        public int DestinationId { get; set; }

        /// <summary>
        /// Número total de reservas para este destino
        /// </summary>
        public int TotalBookings { get; set; }

        /// <summary>
        /// Calificación promedio de todas las reseñas aprobadas (escala 0-5)
        /// </summary>
        public decimal AverageRating { get; set; }

        /// <summary>
        /// Número total de reseñas aprobadas
        /// </summary>
        public int ReviewCount { get; set; }

        /// <summary>
        /// Fecha y hora en que estas estadísticas fueron calculadas por última vez
        /// </summary>
        public DateTime LastCalculatedAt { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación
        /// <summary>
        /// Destino para el cual se calculan estas estadísticas
        /// </summary>
        public Destination? Destination { get; set; }
    }
}
