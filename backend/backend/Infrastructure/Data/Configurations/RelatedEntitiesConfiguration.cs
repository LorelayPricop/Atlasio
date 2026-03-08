using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using backend.Domain.Entities;

namespace backend.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuración de EF Core para la entidad DestinationImage
    /// </summary>
    public class DestinationImageConfiguration : IEntityTypeConfiguration<DestinationImage>
    {
        public void Configure(EntityTypeBuilder<DestinationImage> builder)
        {
            builder.HasKey(img => img.Id);

            builder.Property(img => img.Url)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(img => img.StorageKey)
                .HasMaxLength(200);

            builder.Property(img => img.MimeType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(img => img.CreatedAt)
                .IsRequired();

            // Índices
            builder.HasIndex(img => img.DestinationId);
            builder.HasIndex(img => new { img.DestinationId, img.IsPrimary });
            builder.HasIndex(img => new { img.DestinationId, img.SortOrder });
        }
    }

    /// <summary>
    /// Configuración de EF Core para la entidad Review
    /// </summary>
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.UserId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(r => r.Rating)
                .HasPrecision(3, 2)
                .IsRequired();

            builder.Property(r => r.Comment)
                .HasMaxLength(1000);

            builder.Property(r => r.Status)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.Property(r => r.UpdatedAt)
                .IsRequired();

            // Índices
            builder.HasIndex(r => r.DestinationId);
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => new { r.DestinationId, r.Status });
        }
    }

    /// <summary>
    /// Configuración de EF Core para la entidad Booking
    /// </summary>
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.UserId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(b => b.TravelStart)
                .IsRequired();

            builder.Property(b => b.TravelEnd)
                .IsRequired();

            builder.Property(b => b.BookingStatus)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(b => b.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(b => b.Currency)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.Property(b => b.UpdatedAt)
                .IsRequired();

            // Índices
            builder.HasIndex(b => b.DestinationId);
            builder.HasIndex(b => b.BookingStatus);
            builder.HasIndex(b => new { b.DestinationId, b.BookingStatus });
            builder.HasIndex(b => b.TravelStart);
        }
    }

    /// <summary>
    /// Configuración de EF Core para la entidad DestinationStats
    /// </summary>
    public class DestinationStatsConfiguration : IEntityTypeConfiguration<DestinationStats>
    {
        public void Configure(EntityTypeBuilder<DestinationStats> builder)
        {
            builder.HasKey(s => s.DestinationId);

            builder.Property(s => s.AverageRating)
                .HasPrecision(3, 2)
                .IsRequired();

            builder.Property(s => s.LastCalculatedAt)
                .IsRequired();

            // Índices
            builder.HasIndex(s => s.AverageRating);
            builder.HasIndex(s => s.TotalBookings);
        }
    }
}
