using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using backend.Domain.Entities;

namespace backend.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuración de EF Core para la entidad DestinationType
    /// </summary>
    public class DestinationTypeConfiguration : IEntityTypeConfiguration<DestinationType>
    {
        public void Configure(EntityTypeBuilder<DestinationType> builder)
        {
            // Clave primaria
            builder.HasKey(dt => dt.Id);

            // Propiedades
            builder.Property(dt => dt.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(dt => dt.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(dt => dt.Icon)
                .HasMaxLength(100);

            builder.Property(dt => dt.DisplayOrder)
                .IsRequired();

            builder.Property(dt => dt.IsActive)
                .IsRequired();

            builder.Property(dt => dt.CreatedAt)
                .IsRequired();

            builder.Property(dt => dt.UpdatedAt)
                .IsRequired();

            // Restricción única
            builder.HasIndex(dt => dt.Code)
                .IsUnique();

            // Índices
            builder.HasIndex(dt => dt.IsActive);
            builder.HasIndex(dt => dt.DisplayOrder);

            // Relaciones
            builder.HasMany(dt => dt.Destinations)
                .WithOne(d => d.Type)
                .HasForeignKey(d => d.DestinationTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
