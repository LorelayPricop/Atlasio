using Microsoft.EntityFrameworkCore;
using backend.Domain.Entities;
using backend.Infrastructure.Data.Configurations;

namespace backend.Infrastructure.Data
{
    /// <summary>
    /// Contexto principal de Entity Framework para la aplicación Atlasio
    /// Configurado para soportar tanto base de datos InMemory como SQL Server
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Constructor que recibe las opciones de configuración del contexto
        /// </summary>
        /// <param name="options">Opciones de configuración del DbContext</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tablas de catálogo
        /// <summary>
        /// DbSet para el catálogo de países
        /// </summary>
        public DbSet<Country> Countries { get; set; }

        /// <summary>
        /// DbSet para el catálogo de tipos de destino
        /// </summary>
        public DbSet<DestinationType> DestinationTypes { get; set; }

        /// <summary>
        /// DbSet para el catálogo de ciudades
        /// </summary>
        public DbSet<City> Cities { get; set; }

        // Entidades principales
        /// <summary>
        /// DbSet para la entidad Destination
        /// Representa la tabla de destinos turísticos en la base de datos
        /// </summary>
        public DbSet<Destination> Destinations { get; set; }

        /// <summary>
        /// DbSet para la entidad DestinationImage
        /// </summary>
        public DbSet<DestinationImage> DestinationImages { get; set; }

        /// <summary>
        /// DbSet para la entidad Review
        /// </summary>
        public DbSet<Review> Reviews { get; set; }

        /// <summary>
        /// DbSet para la entidad Booking
        /// </summary>
        public DbSet<Booking> Bookings { get; set; }

        /// <summary>
        /// DbSet para la entidad DestinationStats
        /// </summary>
        public DbSet<DestinationStats> DestinationStats { get; set; }

        /// <summary>
        /// Método llamado durante la creación del modelo para configurar entidades
        /// Aplica configuraciones desde clases de configuración separadas
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo de Entity Framework</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar configuraciones desde clases separadas
            modelBuilder.ApplyConfiguration(new CountryConfiguration());
            modelBuilder.ApplyConfiguration(new DestinationTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CityConfiguration());
            modelBuilder.ApplyConfiguration(new DestinationConfiguration());
            modelBuilder.ApplyConfiguration(new DestinationImageConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            modelBuilder.ApplyConfiguration(new BookingConfiguration());
            modelBuilder.ApplyConfiguration(new DestinationStatsConfiguration());
        }
    }
}
