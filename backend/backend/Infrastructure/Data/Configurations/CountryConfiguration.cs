using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using backend.Domain.Entities;

namespace backend.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuración de EF Core para la entidad Country
    /// </summary>
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            // Clave primaria
            builder.HasKey(c => c.Code);

            // Propiedades
            builder.Property(c => c.Code)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Region)
                .HasMaxLength(50);

            builder.Property(c => c.IsActive)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .IsRequired();

            // Índices
            builder.HasIndex(c => c.IsActive);
            builder.HasIndex(c => c.Region);

            // Relaciones
            builder.HasMany(c => c.Cities)
                .WithOne(city => city.Country)
                .HasForeignKey(city => city.CountryCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Destinations)
                .WithOne(d => d.Country)
                .HasForeignKey(d => d.CountryCode)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
