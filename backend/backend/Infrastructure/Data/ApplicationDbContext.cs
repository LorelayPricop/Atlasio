using Microsoft.EntityFrameworkCore;
using backend.Domain.Entities;

namespace backend.Infrastructure.Data
{
    /// <summary>
    /// Contexto principal de Entity Framework para la aplicación Atlasio
    /// Configurado para usar base de datos en memoria para desarrollo y demos
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

        /// <summary>
        /// DbSet para la entidad Destination
        /// Representa la tabla de destinos turísticos en la base de datos
        /// </summary>
        public DbSet<Destination> Destinations { get; set; }

        /// <summary>
        /// Método llamado durante la creación del modelo para configurar entidades
        /// Nota: Los índices no son necesarios para InMemoryDatabase
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo de Entity Framework</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // No se configuran índices ya que se usa InMemoryDatabase
            // Los índices no tienen efecto en bases de datos en memoria
        }
    }
}
