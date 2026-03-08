namespace backend.Domain.Entities
{
    /// <summary>
    /// Entidad que representa una imagen asociada a un destino
    /// Soporta múltiples imágenes por destino con ordenamiento
    /// </summary>
    public class DestinationImage
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
        /// URL pública de la imagen
        /// </summary>
        public required string Url { get; set; }

        /// <summary>
        /// Clave de almacenamiento para recuperar la imagen del proveedor de almacenamiento
        /// </summary>
        public string? StorageKey { get; set; }

        /// <summary>
        /// Tipo MIME de la imagen (ej: image/jpeg, image/png)
        /// </summary>
        public required string MimeType { get; set; }

        /// <summary>
        /// Tamaño del archivo de imagen en bytes
        /// </summary>
        public long SizeBytes { get; set; }

        /// <summary>
        /// Indica si esta es la imagen principal/portada del destino
        /// Solo una imagen por destino debe tener esta bandera en true
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Orden de visualización para galerías de imágenes
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Fecha y hora en que se subió la imagen
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación
        /// <summary>
        /// Destino al que pertenece esta imagen
        /// </summary>
        public Destination? Destination { get; set; }
    }
}
