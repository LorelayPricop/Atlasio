using backend.Infrastructure.Data;
using backend.Infrastructure.Services;
using backend.Presentation.Middleware;
using backend.Domain.Interfaces;
using backend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
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
    Log.Information("Iniciando HotelBediaX API...");

// ============================================================================
// CONFIGURACIÓN DE SERVICIOS
// ============================================================================

// Agregar controladores MVC con versionado
builder.Services.AddControllers();

// Configurar API Versioning
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("api-version"),
        new QueryStringApiVersionReader("version")
    );
    opt.ReportApiVersions = true;
});

// Configurar API Explorer para Swagger
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
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
    options.UseInMemoryDatabase("HotelBediaXDb"));

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
    c.SwaggerDoc("v1", new() { Title = "HotelBediaX API", Version = "v1.0" });

    // Incluir comentarios XML para documentación
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelBediaX API v1.0");
        c.RoutePrefix = "swagger"; // Swagger UI estará disponible en /swagger
        c.DocumentTitle = "HotelBediaX API Documentation";
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
    Log.Fatal(ex, "Error fatal al iniciar la aplicación HotelBediaX");
}
finally
{
    Log.CloseAndFlush();
}

// Hacer la clase Program pública para las pruebas de integración
public partial class Program { }
