# Atlasio Backend API - .NET 10

API REST para gestión de destinos turísticos construida con **.NET 10** siguiendo **Clean Architecture** y patrones CQRS.

---

## Tabla de Contenidos

- [Quick Start](#-quick-start)
- [Arquitectura](#-arquitectura)
- [Endpoints](#-endpoints)
- [Modelo de Datos](#-modelo-de-datos)
- [Configuración](#-configuración)
- [Testing](#-testing)

---

## Quick Start

### Prerrequisitos
- **.NET 10 SDK** ([Descargar](https://dotnet.microsoft.com/download/dotnet/10.0))
- Visual Studio 2022 / VS Code / Rider

### Ejecutar

```bash
# Restaurar paquetes
dotnet restore

# Compilar
dotnet build

# Ejecutar
dotnet run

# Ejecutar con watch (hot reload)
dotnet watch run
```

### Acceso

- **API Base**: `https://localhost:5001/api/v1`
- **Swagger UI**: `https://localhost:5001/swagger`
- **Health Check**: `https://localhost:5001/health`

---

## Arquitectura

### Clean Architecture (Hexagonal)

```
┌──────────────────────────────────────────┐
│         PRESENTATION LAYER               │
│  (Controllers, Middleware, API Versioning)│
└──────────────────────────────────────────┘
                    ↓
┌──────────────────────────────────────────┐
│        APPLICATION LAYER                  │
│  (Commands, Queries, DTOs, Handlers)     │
└──────────────────────────────────────────┘
                    ↓
┌──────────────────────────────────────────┐
│          DOMAIN LAYER                     │
│    (Entities, Interfaces, Business Rules)│
└──────────────────────────────────────────┘
                    ↑
┌──────────────────────────────────────────┐
│       INFRASTRUCTURE LAYER                │
│  (EF Core, Repositories, External Services)│
└──────────────────────────────────────────┘
```

### Estructura del Proyecto

```
backend/
├── Domain/                      # Núcleo del negocio (sin dependencias)
│   ├── Entities/                # Entidades del dominio
│   │   ├── Destination.cs       # Destino turístico
│   │   ├── Country.cs           # Catálogo: Países
│   │   ├── DestinationType.cs   # Catálogo: Tipos de destino
│   │   ├── City.cs              # Catálogo: Ciudades
│   │   ├── DestinationImage.cs  # Imágenes (1:N)
│   │   ├── Review.cs            # Reseñas (1:N)
│   │   ├── Booking.cs           # Reservas (1:N)
│   │   └── DestinationStats.cs  # Estadísticas (1:1)
│   └── Interfaces/              # Contratos (Ports)
│       ├── IRepository.cs       # Repositorio genérico
│       ├── IDestinationRepository.cs
│       ├── IRepositoryManager.cs
│       ├── IFilterCriteria.cs
│       └── IDestinationAdapter.cs
│
├── Application/                 # Casos de uso (lógica de aplicación)
│   ├── Commands/                # CQRS - Escritura
│   │   ├── CreateDestinationCommand.cs
│   │   ├── UpdateDestinationCommand.cs
│   │   └── DeleteDestinationCommand.cs
│   ├── Queries/                 # CQRS - Lectura
│   │   ├── GetDestinationsQuery.cs
│   │   ├── GetDestinationByIdQuery.cs
│   │   └── GetDestinationTypesQuery.cs
│   ├── DTOs/                    # Data Transfer Objects
│   │   ├── DestinationDto.cs
│   │   ├── CatalogDtos.cs
│   │   └── PagedResultDto.cs
│   └── Adapters/                # Mapeo manual (sin AutoMapper)
│       ├── DestinationMapper.cs
│       ├── DestinationAdapter.cs
│       └── DestinationFilterAdapter.cs
│
├── Infrastructure/              # Implementaciones técnicas
│   ├── Data/                    # Entity Framework Core
│   │   ├── ApplicationDbContext.cs
│   │   └── Configurations/      # Fluent API configurations
│   │       ├── DestinationConfiguration.cs
│   │       ├── CountryConfiguration.cs
│   │       ├── DestinationTypeConfiguration.cs
│   │       ├── CityConfiguration.cs
│   │       └── RelatedEntitiesConfiguration.cs
│   ├── Repositories/            # Implementación de repositorios
│   │   ├── Repository.cs        # Genérico
│   │   └── DestinationRepository.cs
│   ├── UnitOfWork/
│   │   └── RepositoryManager.cs
│   └── Services/
│       └── DataSeedService.cs   # Seed de datos inicial
│
└── Presentation/                # API REST
    ├── Controllers/
    │   ├── DestinationsController.cs  # CRUD de destinos
    │   └── CatalogController.cs       # Endpoints de catálogos
    └── Middleware/
        └── GlobalExceptionMiddleware.cs
```

### Patrones Implementados

| Patrón | Propósito | Ubicación |
|--------|-----------|-----------|
| **CQRS** | Separar lecturas de escrituras | Application/Commands, Application/Queries |
| **Mediator** | Desacoplar handlers de controllers | MediatR |
| **Repository** | Abstracción de acceso a datos | Infrastructure/Repositories |
| **Unit of Work** | Gestionar transacciones | Infrastructure/UnitOfWork |
| **Adapter** | Conversión entre capas | Application/Adapters |
| **Dependency Injection** | Inversión de control | ASP.NET Core DI |

---

## 📡 Endpoints

### Destinos (`DestinationsController`)

| Método | Ruta | Descripción | Body/Params |
|--------|------|-------------|-------------|
| `GET` | `/api/v1/destinations` | Lista paginada con filtros | `?searchTerm`, `?countryCode`, `?destinationTypeId`, `?page`, `?pageSize` |
| `GET` | `/api/v1/destinations/{id}` | Obtener por ID | - |
| `POST` | `/api/v1/destinations` | Crear destino | `CreateDestinationDto` |
| `PUT` | `/api/v1/destinations/{id}` | Actualizar destino | `UpdateDestinationDto` |
| `DELETE` | `/api/v1/destinations/{id}` | Eliminar destino | - |
| `GET` | `/api/v1/destinations/countries` | Códigos de países únicos | - |
| `GET` | `/api/v1/destinations/types` | Tipos de destino | - |

### Catálogos (`CatalogController`) 

| Método | Ruta | Descripción | Params |
|--------|------|-------------|--------|
| `GET` | `/api/v1/catalog/countries` | Catálogo completo de países | `?onlyActive` |
| `GET` | `/api/v1/catalog/countries/{code}` | País específico | - |
| `GET` | `/api/v1/catalog/destination-types` | Catálogo de tipos | `?onlyActive` |
| `GET` | `/api/v1/catalog/destination-types/{id}` | Tipo específico | - |
| `GET` | `/api/v1/catalog/cities` | Catálogo de ciudades | `?countryCode`, `?onlyActive` |

### Ejemplos de Request

#### Crear Destino
```json
POST /api/v1/destinations
Content-Type: application/json

{
  "name": "Barcelona",
  "description": "Ciudad cosmopolita con arquitectura única",
  "longDescription": "Barcelona es famosa por la arquitectura de Gaudí...",
  "countryCode": "ESP",
  "destinationTypeId": 4,
  "imageUrl": "https://example.com/barcelona.jpg"
}
```

#### Filtrar Destinos
```bash
GET /api/v1/destinations?searchTerm=playa&countryCode=MEX&page=1&pageSize=10
```

#### Obtener Tipos de Destino
```bash
GET /api/v1/catalog/destination-types

# Respuesta:
[
  {
    "id": 1,
    "code": "BEACH",
    "name": "Beach",
    "icon": "beach_access",
    "displayOrder": 1,
    "isActive": true
  },
  ...
]
```

---

## Modelo de Datos

### Diagrama de Relaciones

```
┌─────────────┐        ┌──────────────────┐
│  Country    │◄──────┤   Destination    │
│  (Catálogo) │        │   (Principal)    │
└─────────────┘        └──────────────────┘
                              │ 1
                              │
         ┌────────────────────┼────────────────────┐
         │                    │                    │
         ▼ N                  ▼ N                  ▼ N
┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐
│ DestinationImage │  │     Review       │  │     Booking      │
└──────────────────┘  └──────────────────┘  └──────────────────┘

         ┌────────────────────┼────────────────────┐
         │                    │                    │
         ▼ 1                  ▼ 1                  ▼ 1
┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐
│ DestinationType  │  │ DestinationStats │  │      City        │
│   (Catálogo)     │  │   (Agregado)     │  │   (Catálogo)     │
└──────────────────┘  └──────────────────┘  └──────────────────┘
```

### Entidad Principal: `Destination`

```csharp
public class Destination
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? LongDescription { get; set; }
    public string CountryCode { get; set; }         // FK
    public int DestinationTypeId { get; set; }      // FK
    public string Status { get; set; }
    public DateTime LastModif { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    
    // Campos obsoletos (migrar a entidades relacionadas)
    [Obsolete] public string? ImageUrl { get; set; }
    [Obsolete] public int TotalBookings { get; set; }
    [Obsolete] public decimal AverageRating { get; set; }
    [Obsolete] public int ReviewCount { get; set; }
    
    // Relaciones
    public Country? Country { get; set; }
    public DestinationType? Type { get; set; }
    public ICollection<DestinationImage> Images { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<Booking> Bookings { get; set; }
    public DestinationStats? Stats { get; set; }
}
```

### Catálogos

#### Country (Países)
```csharp
public class Country
{
    public string Code { get; set; }    // PK (3 chars: ESP, MEX, USA)
    public string Name { get; set; }
    public string? Region { get; set; }
    public bool IsActive { get; set; } = true;
}
```

#### DestinationType (Tipos de Destino)
```csharp
public class DestinationType
{
    public int Id { get; set; }         // PK
    public string Code { get; set; }    // BEACH, MOUNTAIN, CITY...
    public string Name { get; set; }
    public string? Icon { get; set; }   // Material icon name
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
```

#### City (Ciudades)
```csharp
public class City
{
    public int Id { get; set; }
    public string CountryCode { get; set; }  // FK
    public string Name { get; set; }
    public bool IsActive { get; set; } = true;
}
```

---

## Configuración

### appsettings.json

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/atlasio-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        }
      }
    ]
  },
  "AllowedHosts": "*"
}
```

### CORS

Configurado para aceptar requests desde:
- `http://localhost:4200` (Angular)

Para agregar más orígenes, modificar en `Program.cs`:

```csharp
policy.WithOrigins("http://localhost:4200", "https://mi-dominio.com")
```

### Base de Datos

**InMemory Database** (desarrollo):
```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("AtlasioDb"));
```

**Migrar a SQL Server** (producción):
```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```

---

## Testing

Ejecutar tests del backend:

```bash
cd ../backend.Tests
dotnet test

# Con cobertura
dotnet test /p:CollectCoverage=true

# Verbose
dotnet test --logger "console;verbosity=detailed"
```

Ver [backend.Tests/README.md](../backend.Tests/README.md) para más detalles.

---

## Paquetes NuGet

| Paquete | Versión | Propósito |
|---------|---------|-----------|
| `Microsoft.AspNetCore.OpenApi` | 10.0.3 | OpenAPI/Swagger |
| `Swashbuckle.AspNetCore` | 10.1.4 | Swagger UI |
| `Swashbuckle.AspNetCore.Annotations` | 10.1.4 | Anotaciones Swagger |
| `Microsoft.EntityFrameworkCore.InMemory` | 10.0.3 | Base de datos en memoria |
| `Serilog.AspNetCore` | 10.0.0 | Logging estructurado |
| `MediatR` | 14.1.0 | CQRS/Mediator |
| `Asp.Versioning.Mvc` | 8.1.1 | Versionado de API |

---

## Extensibilidad

### Agregar una Nueva Entidad

1. **Crear entidad** en `Domain/Entities/`
2. **Agregar configuración** en `Infrastructure/Data/Configurations/`
3. **Registrar DbSet** en `ApplicationDbContext.cs`
4. **Crear DTOs** en `Application/DTOs/`
5. **Crear Commands/Queries** en `Application/`
6. **Crear Repository** (si es necesario) en `Infrastructure/Repositories/`
7. **Crear Controller** en `Presentation/Controllers/`

### Agregar un Nuevo Endpoint

1. Crear **Query/Command** en `Application/`
2. Crear **Handler** correspondiente
3. Agregar **endpoint** en Controller
4. Documentar con **XML comments**
5. Actualizar **Swagger** examples

---

## Notas de Desarrollo

### Cambios Recientes (v1.0)

✅ **Migración de Enum a Entidad**
- `DestinationType` es ahora una entidad de catálogo
- Usar `DestinationTypeId` (int) en lugar de enum
- Ver endpoint `/api/v1/catalog/destination-types` para IDs

✅ **Nuevos Endpoints de Catálogo**
- `/api/v1/catalog/*` para países, tipos y ciudades
- Soporte para filtro `onlyActive`

✅ **Swagger Mejorado**
- Documentación XML completa
- Ejemplos de request/response
- Descripciones detalladas

### Próximas Mejoras

- [ ] Autenticación JWT
- [ ] Rate limiting
- [ ] Cache con Redis
- [ ] Upload de imágenes
- [ ] Notificaciones
- [ ] Métricas y observabilidad

---

## Recursos

- [Documentación .NET 10](https://docs.microsoft.com/dotnet/)
- [Clean Architecture Guide](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [Swagger OpenAPI](https://swagger.io/specification/)

---

**Desarrollado con ❤️ usando .NET 10 y Clean Architecture**
