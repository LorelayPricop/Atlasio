using backend.Infrastructure.Data;
using backend.Infrastructure.Services;
using backend.Presentation.Middleware;
using backend.Domain.Interfaces;
using backend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using MediatR;

// Configuración y construcción de la aplicación web
var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithProcessId()
        .Enrich.WithThreadId());

try
{
    Log.Information("Iniciando Atlasio API...");

// ============================================================================
// CONFIGURACIÓN DE SERVICIOS
// ============================================================================

// Agregar controladores MVC con versionado
builder.Services.AddControllers();

// Configurar API Versioning (nueva sintaxis Asp.Versioning 8.x)
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("api-version"),
        new QueryStringApiVersionReader("version")
    );
}).AddMvc()
  .AddApiExplorer(options =>
  {
      options.GroupNameFormat = "'v'VVV";
      options.SubstituteApiVersionInUrl = true;
  });

// Configurar CORS para permitir comunicación con el frontend Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            // Permitir solo el origen del frontend Angular
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()      // Permitir cualquier header HTTP
                  .AllowAnyMethod();     // Permitir cualquier método HTTP (GET, POST, PUT, DELETE)
        });
});

// Configurar Entity Framework Core con base de datos en memoria
// Esta es la implementación del "mock database" solicitado en la prueba técnica
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("AtlasioDb"));

// Registrar adaptadores para conversión entre entidades y DTOs
builder.Services.AddScoped<backend.Domain.Interfaces.IDestinationAdapter, backend.Application.Adapters.DestinationAdapter>();

// Configurar MediatR para CQRS
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Registrar repositorios
builder.Services.AddScoped<IDestinationRepository, DestinationRepository>();

// Registrar Unit of Work
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

// Registrar servicios de la aplicación
builder.Services.AddScoped<DataSeedService>();                          // Servicio para poblar datos de ejemplo

// Configurar Swagger/OpenAPI para documentación de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Atlasio API - Destinos Turísticos", 
        Version = "v1.0",
        Description = "API RESTful para gestión de destinos turísticos con arquitectura hexagonal y CQRS.\n\n" +
                      "**Características principales:**\n" +
                      "- CRUD completo de destinos turísticos\n" +
                      "- Catálogos de países, tipos de destino y ciudades\n" +
                      "- Filtros avanzados y paginación\n" +
                      "- Base de datos InMemory para desarrollo\n" +
                      "- Validación de datos con DataAnnotations\n\n" +
                      "**Arquitectura:**\n" +
                      "- Clean Architecture / Hexagonal Architecture\n" +
                      "- CQRS con MediatR\n" +
                      "- Repository Pattern y Unit of Work\n" +
                      "- Entity Framework Core InMemory\n\n" +
                      "**Nota importante:** Los tipos de destino ahora se manejan mediante catálogo (DestinationTypeId) " +
                      "en lugar de enum. Usa el endpoint `/api/v1/catalog/destination-types` para obtener los IDs disponibles.",
        Contact = new()
        {
            Name = "Atlasio Development Team",
            Email = "dev@atlasio.com"
        }
    });

    // Incluir comentarios XML para documentación
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }

    // Ordenar endpoints por nombre de controlador
    c.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.RelativePath}");

    // Configurar ejemplos de respuesta
    c.EnableAnnotations();
});

// ============================================================================
// CONSTRUCCIÓN Y CONFIGURACIÓN DE LA APLICACIÓN
// ============================================================================

var app = builder.Build();

// Ejecutar seed de datos al iniciar la aplicación
// Solo se ejecuta si la base de datos está vacía
using (var scope = app.Services.CreateScope())
{
    var seedService = scope.ServiceProvider.GetRequiredService<DataSeedService>();
    await seedService.SeedDataAsync();
}

// ============================================================================
// CONFIGURACIÓN DEL PIPELINE HTTP
// ============================================================================

// Configurar middleware de manejo de excepciones globales
app.UseGlobalExceptionMiddleware();

// Configurar Swagger/OpenAPI solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Atlasio API v1.0");
        c.RoutePrefix = "swagger"; // Swagger UI estará disponible en /swagger
        c.DocumentTitle = "Atlasio API Documentation";
    });
}

// Redirigir HTTP a HTTPS para mayor seguridad
app.UseHttpsRedirection();

// Aplicar política CORS configurada anteriormente
app.UseCors("AllowAngularApp");

// Agregar middleware de autorización
app.UseAuthorization();

// Mapear controladores de la API
app.MapControllers();

// ============================================================================
// INICIAR LA APLICACIÓN
// ============================================================================

app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Error fatal al iniciar la aplicación Atlasio");
}
finally
{
    Log.CloseAndFlush();
}

// Hacer la clase Program pública para las pruebas de integración
public partial class Program { }
