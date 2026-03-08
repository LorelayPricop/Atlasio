using System.ComponentModel.DataAnnotations;

namespace backend.Application.DTOs
{
    /// <summary>
    /// DTO para transferir información completa de un destino
    /// Se usa para respuestas de la API
    /// </summary>
    public class DestinationDto
    {
        /// <summary>Identificador único del destino</summary>
        public required int ID { get; init; }
        /// <summary>Nombre del destino turístico</summary>
        public required string Name { get; init; }
        /// <summary>Descripción detallada del destino</summary>
        public required string Description { get; init; }
        /// <summary>Descripción larga y detallada del destino con información completa</summary>
        public string? LongDescription { get; init; }
        /// <summary>Código ISO del país (3 caracteres)</summary>
        public required string CountryCode { get; init; }
        /// <summary>ID del tipo de destino turístico</summary>
        public required int DestinationTypeId { get; init; }
        /// <summary>Nombre del tipo de destino (para mostrar en UI)</summary>
        public string? TypeName { get; init; }
        /// <summary>Fecha de última modificación</summary>
        public required DateTime LastModif { get; init; }
        /// <summary>URL de la imagen principal del destino</summary>
        public string? ImageUrl { get; init; }
        /// <summary>Total de reservas del destino</summary>
        public int TotalBookings { get; init; }
        /// <summary>Calificación promedio (escala 0-5)</summary>
        public decimal AverageRating { get; init; }
        /// <summary>Número de reseñas</summary>
        public int ReviewCount { get; init; }
        /// <summary>Estado del destino (Active, Inactive)</summary>
        public string Status { get; init; } = "Active";
        /// <summary>Nombre del usuario que creó el registro</summary>
        public string? CreatedBy { get; init; }
        /// <summary>Fecha de creación del registro</summary>
        public required DateTime CreatedDate { get; init; }
    }

    /// <summary>
    /// DTO para crear un nuevo destino
    /// No incluye ID ni LastModif ya que se generan automáticamente
    /// </summary>
    public class CreateDestinationDto
    {
        /// <summary>Nombre del destino turístico</summary>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
        public required string Name { get; init; }

        /// <summary>Descripción detallada del destino</summary>
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 500 characters")]
        public required string Description { get; init; }

        /// <summary>Descripción larga y detallada del destino con información completa</summary>
        [StringLength(2000, ErrorMessage = "Long description must not exceed 2000 characters")]
        public string? LongDescription { get; init; }

        /// <summary>URL o data URL de la imagen principal del destino</summary>
        public string? ImageUrl { get; init; }

        /// <summary>Código ISO del país (3 caracteres)</summary>
        [Required(ErrorMessage = "Country code is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Country code must be exactly 3 characters")]
        public required string CountryCode { get; init; }

        /// <summary>ID del tipo de destino turístico</summary>
        [Required(ErrorMessage = "Destination type is required")]
        public required int DestinationTypeId { get; init; }
    }

    /// <summary>
    /// DTO para actualizar un destino existente
    /// No incluye ID ni LastModif ya que se manejan internamente
    /// </summary>
    public class UpdateDestinationDto
    {
        /// <summary>Nombre del destino turístico</summary>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
        public required string Name { get; init; }

        /// <summary>Descripción detallada del destino</summary>
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 500 characters")]
        public required string Description { get; init; }

        /// <summary>Descripción larga y detallada del destino con información completa</summary>
        [StringLength(2000, ErrorMessage = "Long description must not exceed 2000 characters")]
        public string? LongDescription { get; init; }

        /// <summary>URL o data URL de la imagen principal del destino</summary>
        public string? ImageUrl { get; init; }

        /// <summary>Código ISO del país (3 caracteres)</summary>
        [Required(ErrorMessage = "Country code is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Country code must be exactly 3 characters")]
        public required string CountryCode { get; init; }

        /// <summary>ID del tipo de destino turístico</summary>
        [Required(ErrorMessage = "Destination type is required")]
        public required int DestinationTypeId { get; init; }
    }

    /// <summary>
    /// DTO para filtrar y paginar la lista de destinos
    /// Todos los campos son opcionales para permitir filtros flexibles
    /// </summary>
    public class DestinationFilterDto
    {
        /// <summary>Término de búsqueda que se aplica a nombre, descripción y código de país</summary>
        public string? SearchTerm { get; set; }
        /// <summary>Filtro por código de país específico</summary>
        public string? CountryCode { get; set; }
        /// <summary>Filtro por ID de tipo de destino específico</summary>
        public int? DestinationTypeId { get; set; }
        /// <summary>Número de página actual (comienza en 1)</summary>
        public int Page { get; set; } = 1;
        /// <summary>Número de elementos por página (máximo 100 recomendado)</summary>
        public int PageSize { get; set; } = 20;
    }

    /// <summary>
    /// DTO genérico para resultados paginados
    /// Se puede usar con cualquier tipo de entidad
    /// </summary>
    /// <typeparam name="T">Tipo de entidad que se está paginando</typeparam>
    public class PagedResultDto<T>
    {
        /// <summary>Lista de elementos de la página actual</summary>
        public List<T> Items { get; init; } = [];
        /// <summary>Número total de elementos que coinciden con los filtros</summary>
        public required int TotalCount { get; init; }
        /// <summary>Número de página actual</summary>
        public required int Page { get; init; }
        /// <summary>Número de elementos por página</summary>
        public required int PageSize { get; init; }
        /// <summary>Número total de páginas calculado automáticamente</summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
