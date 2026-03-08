using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using backend.Domain.Entities;

namespace backend.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuración de EF Core para la entidad City
    /// </summary>
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            // Clave primaria
            builder.HasKey(c => c.Id);

            // Propiedades
            builder.Property(c => c.CountryCode)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.IsActive)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .IsRequired();

            // Índices
            builder.HasIndex(c => c.CountryCode);
            builder.HasIndex(c => c.IsActive);
            builder.HasIndex(c => new { c.CountryCode, c.Name });

            // Relaciones
            builder.HasOne(c => c.Country)
                .WithMany(country => country.Cities)
                .HasForeignKey(c => c.CountryCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Destinations)
                .WithOne(d => d.City)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
