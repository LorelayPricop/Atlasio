using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using backend.Domain.Entities;

namespace backend.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuración de EF Core para la entidad Destination
    /// </summary>
    public class DestinationConfiguration : IEntityTypeConfiguration<Destination>
    {
        public void Configure(EntityTypeBuilder<Destination> builder)
        {
            // Clave primaria
            builder.HasKey(d => d.ID);

            // Propiedades
            builder.Property(d => d.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(d => d.Description)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(d => d.LongDescription)
                .HasMaxLength(2000);

            builder.Property(d => d.CountryCode)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(d => d.DestinationTypeId)
                .IsRequired();

            builder.Property(d => d.Status)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(d => d.CreatedBy)
                .HasMaxLength(100);

            builder.Property(d => d.LastModif)
                .IsRequired();

            builder.Property(d => d.CreatedDate)
                .IsRequired();

            // Índices
            builder.HasIndex(d => d.CountryCode);
            builder.HasIndex(d => d.DestinationTypeId);
            builder.HasIndex(d => d.CityId);
            builder.HasIndex(d => new { d.Status, d.LastModif });
            builder.HasIndex(d => d.Name);
            builder.HasIndex(d => new { d.Name, d.Description });

            // Relaciones
            builder.HasOne(d => d.Country)
                .WithMany(c => c.Destinations)
                .HasForeignKey(d => d.CountryCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Type)
                .WithMany(dt => dt.Destinations)
                .HasForeignKey(d => d.DestinationTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.City)
                .WithMany(c => c.Destinations)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(d => d.Images)
                .WithOne(img => img.Destination)
                .HasForeignKey(img => img.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Reviews)
                .WithOne(r => r.Destination)
                .HasForeignKey(r => r.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Bookings)
                .WithOne(b => b.Destination)
                .HasForeignKey(b => b.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Stats)
                .WithOne(s => s.Destination)
                .HasForeignKey<DestinationStats>(s => s.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
