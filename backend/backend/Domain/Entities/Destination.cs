namespace backend.Domain.Entities
{
    /// <summary>
    /// Main model representing a tourist destination
    /// Now uses relational foreign keys instead of enums for better scalability
    /// </summary>
    public class Destination
    {
        /// <summary>
        /// Unique destination identifier (primary key)
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tourist destination name
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Detailed description of the destination
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Long and detailed description of the destination with complete information
        /// </summary>
        public string? LongDescription { get; set; }

        /// <summary>
        /// ISO country code (3-character format: MEX, USA, ESP, etc.)
        /// Foreign key to Countries table
        /// </summary>
        public required string CountryCode { get; set; }

        /// <summary>
        /// Foreign key to DestinationType
        /// Replaces the old DestinationType enum
        /// </summary>
        public int DestinationTypeId { get; set; }

        /// <summary>
        /// Optional foreign key to City
        /// </summary>
        public int? CityId { get; set; }

        /// <summary>
        /// Date and time of the last modification of the record
        /// Automatically updated on each write operation
        /// </summary>
        public DateTime LastModif { get; set; }

        /// <summary>
        /// URL of the main destination image (deprecated - use DestinationImages)
        /// Kept for backward compatibility
        /// </summary>
        [Obsolete("Use DestinationImages collection instead")]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Total bookings for the destination (deprecated - use DestinationStats)
        /// Kept for backward compatibility
        /// </summary>
        [Obsolete("Use DestinationStats.TotalBookings instead")]
        public int TotalBookings { get; set; }

        /// <summary>
        /// Average rating (0-5 scale) (deprecated - use DestinationStats)
        /// Kept for backward compatibility
        /// </summary>
        [Obsolete("Use DestinationStats.AverageRating instead")]
        public decimal AverageRating { get; set; }

        /// <summary>
        /// Number of reviews (deprecated - use DestinationStats)
        /// Kept for backward compatibility
        /// </summary>
        [Obsolete("Use DestinationStats.ReviewCount instead")]
        public int ReviewCount { get; set; }

        /// <summary>
        /// Destination status (Active, Inactive, Draft)
        /// </summary>
        public string Status { get; set; } = "Active";

        /// <summary>
        /// Name of the user who created the record
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Record creation date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        /// <summary>
        /// Country to which this destination belongs
        /// </summary>
        public Country? Country { get; set; }

        /// <summary>
        /// Type of tourist destination
        /// </summary>
        public DestinationType? Type { get; set; }

        /// <summary>
        /// City where this destination is located (optional)
        /// </summary>
        public City? City { get; set; }

        /// <summary>
        /// Images associated with this destination
        /// </summary>
        public ICollection<DestinationImage> Images { get; set; } = [];

        /// <summary>
        /// Reviews for this destination
        /// </summary>
        public ICollection<Review> Reviews { get; set; } = [];

        /// <summary>
        /// Bookings for this destination
        /// </summary>
        public ICollection<Booking> Bookings { get; set; } = [];

        /// <summary>
        /// Aggregated statistics for this destination
        /// </summary>
        public DestinationStats? Stats { get; set; }
    }
}
