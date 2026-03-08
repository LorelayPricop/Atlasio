# HotelBediaX Backend - API REST

API REST desarrollada con **.NET 10 (LTS)** para la gestión de destinos turísticos, implementando **Arquitectura Hexagonal** con patrones CQRS, Repository y Unit of Work.

## Inicio Rápido

### Prerrequisitos
- **.NET 10 SDK** (LTS - soporte hasta noviembre 2028)
- Visual Studio 2022 o VS Code

### Ejecutar la Aplicación
```bash
cd backend/backend
dotnet restore
dotnet run
```

### Acceso
- **API**: `http://localhost:5259/api/v1/destinations`
- **Swagger**: `http://localhost:5259/swagger`

## Arquitectura Hexagonal

### Estructura del Proyecto

```
backend/
├── Domain/               # DOMINIO - Lógica de negocio pura
│   ├── Entities/         # Entidades del dominio
│   ├── Enums/           # Enumeraciones
│   └── Interfaces/      # Interfaces del dominio (Ports)
├── Application/          # APLICACIÓN - Casos de uso y reglas
│   ├── Commands/        # CQRS - Comandos para escritura
│   ├── Queries/         # CQRS - Queries para lectura
│   ├── DTOs/            # Objetos de transferencia
│   └── Adapters/        # Adaptadores manuales para mapeo
├── Infrastructure/       # INFRAESTRUCTURA - Persistencia
│   ├── Data/            # Contexto Entity Framework
│   ├── Repositories/    # Repository Pattern (Adapters)
│   ├── Services/        # Servicios de infraestructura
│   └── UnitOfWork/      # Unit of Work Pattern
└── Presentation/         # PRESENTACIÓN - API y middleware
    ├── Controllers/     # Controladores de la API
    └── Middleware/     # Middleware personalizado
```

### Flujo de Dependencias

```
Presentation → Application → Domain
     ↓              ↓
Infrastructure → Application → Domain
```

**Regla**: Las dependencias siempre apuntan hacia el centro (Domain).

### Decisiones Arquitectónicas

#### Arquitectura Optimizada

La estructura actual sigue **Clean Architecture** con **CQRS** usando **MediatR**:

- **DTOs centralizados** en `Application/DTOs` para reutilización entre Commands, Queries y Controllers
- **Interfaces en Domain** para mantener la inversión de dependencias
- **Handlers como servicios** en lugar de servicios tradicionales
- **Controladores limpios** que solo delegan a MediatR

### Beneficios

- **Testabilidad**: Domain y Application fáciles de testear
- **Independencia**: El dominio no depende de frameworks
- **Flexibilidad**: Fácil cambio de implementaciones
- **Mantenibilidad**: Separación clara de responsabilidades
- **Escalabilidad**: Cada capa evoluciona independientemente

## Modelo de Datos

### Entidad Principal: `Destination`

```csharp
public class Destination
{
    public int ID { get; set; }                    // Identificador único
    public string Name { get; set; }               // Nombre del destino
    public string Description { get; set; }        // Descripción detallada
    public string CountryCode { get; set; }        // Código ISO del país (3 chars)
    public DestinationType Type { get; set; }      // Tipo de destino
    public DateTime LastModif { get; set; }        // Última modificación
}
```

### Tipos de Destino

```csharp
public enum DestinationType
{
    Beach,      // Destinos de playa y costa
    Mountain,   // Destinos de montaña
    City,       // Destinos urbanos
    Cultural,   // Patrimonio cultural e histórico
    Adventure,  // Actividades de aventura
    Relax       // Destinos de relajación
}
```

## API Endpoints

### Operaciones CRUD

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/v1/destinations` | Lista paginada con filtros |
| `GET` | `/api/v1/destinations/{id}` | Obtener destino por ID |
| `POST` | `/api/v1/destinations` | Crear nuevo destino |
| `PUT` | `/api/v1/destinations/{id}` | Actualizar destino |
| `DELETE` | `/api/v1/destinations/{id}` | Eliminar destino |

### Endpoints de Soporte

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/v1/destinations/countries` | Lista de códigos de países |
| `GET` | `/api/v1/destinations/types` | Lista de tipos de destino |

### Filtros Disponibles

```http
# Búsqueda por texto
GET /api/v1/destinations?searchTerm=playa

# Filtro por país
GET /api/v1/destinations?countryCode=MEX

# Filtro por tipo
GET /api/v1/destinations?type=Beach

# Combinación de filtros
GET /api/v1/destinations?searchTerm=playa&countryCode=MEX&type=Beach&page=1&pageSize=10
```

### Versionado de API

La API soporta múltiples métodos de versionado:

1. **URL Path**: `/api/v1/destinations` (recomendado)
2. **Query String**: `?version=1.0`
3. **Header**: `api-version: 1.0`

## Tecnologías

### Core
- **.NET 10 (LTS)**: Framework de desarrollo con soporte a largo plazo hasta noviembre 2028
- **ASP.NET Core 10**: Framework web para APIs REST
- **Entity Framework Core 10**: ORM para acceso a datos
- **Base de Datos en Memoria**: Mock database para demostración

### Patrones y Librerías
- **MediatR 14.1**: Implementación de CQRS y patrón Mediator con DI integrado
- **Adaptadores manuales**: Mapeo explícito entre entidades y DTOs
- **Repository Pattern**: Abstracción del acceso a datos
- **Unit of Work**: Coordinación de transacciones

### Documentación y Logging
- **Swagger/OpenAPI 10**: Documentación automática de la API
- **Serilog 10**: Logging estructurado con múltiples sinks

## Datos de Ejemplo

El sistema incluye **10 destinos turísticos reales**:

- **Playa del Carmen** (MEX) - Beach
- **Santorini** (GRC) - Cultural  
- **Kyoto** (JPN) - Cultural
- **Machu Picchu** (PER) - Adventure
- **París** (FRA) - City
- **Nueva York** (USA) - City
- **Barcelona** (ESP) - Cultural
- **Río de Janeiro** (BRA) - City
- **Alpes Suizos** (CHE) - Mountain
- **Bali** (IDN) - Relax

## Testing

Para información detallada sobre testing, cobertura y ejecución de tests, consulta el [README del proyecto de tests](../backend.Tests/README.md).

## Configuración

### Paquetes NuGet Principales

**Core ASP.NET:**
- Microsoft.AspNetCore.OpenApi (10.0.3)
- Swashbuckle.AspNetCore (10.1.4)
- Swashbuckle.AspNetCore.SwaggerGen (10.1.4)

**Entity Framework:**
- Microsoft.EntityFrameworkCore.InMemory (10.0.3)

**CQRS and Mediator:**
- MediatR (14.1.0) - *Con DI integrado*

**Logging:**
- Serilog.AspNetCore (10.0.0)
- Serilog.Sinks.Console (6.1.1)
- Serilog.Sinks.File (7.0.0)
- Serilog.Enrichers.Environment (3.0.1)
- Serilog.Enrichers.Process (3.0.0)
- Serilog.Enrichers.Thread (4.0.0)
- Serilog.Settings.Configuration (10.0.0)

**API Versioning:**
- Asp.Versioning.Mvc (8.1.1)
- Asp.Versioning.Mvc.ApiExplorer (8.1.1)

### Configuración de Servicios

- **CORS**: Configurado para Angular (puerto 4200)
- **Entity Framework**: Base de datos en memoria con índices optimizados
- **Adaptadores manuales**: Mapeo explícito entre entidades y DTOs
- **MediatR**: Configuración integrada con DI nativo de .NET 10
- **API Versioning**: Configuración moderna con Asp.Versioning 8.x
- **Swagger**: Documentación automática de la API con OpenAPI 10
- **Middleware**: Manejo global de excepciones

## Características de Rendimiento

### Optimizaciones Implementadas

- **Índices de base de datos** en campos de filtrado frecuente
- **Paginación eficiente** para grandes volúmenes de datos
- **Consultas optimizadas** con Entity Framework Core 10
- **Manejo asíncrono** de todas las operaciones
- **Middleware optimizado** para manejo de errores
- **Logging estructurado** para monitoreo

### Mejoras de .NET 10 (LTS)

- **Rendimiento mejorado**: Mayor velocidad de ejecución y menor consumo de memoria
- **Nuevas características de C# 13**: Sintaxis mejorada y mejor expresividad
- **Soporte a largo plazo**: Actualizaciones de seguridad garantizadas hasta noviembre 2028
- **Mejor integración con contenedores**: Optimizaciones para Docker y Kubernetes
- **AOT (Ahead-of-Time) compilation**: Opción de compilación nativa para mejor rendimiento

### Capacidad de Escalabilidad

- **Diseñado para 200k+ registros** como especifica la prueba técnica
- **Filtrado eficiente** por múltiples criterios
- **Paginación configurable** para diferentes tamaños de página
- **Arquitectura preparada** para migración a base de datos real
- **Manejo de errores robusto** para alta disponibilidad

## Troubleshooting

### Error: "Unable to bind to https://localhost:5259"
- Verificar que el puerto 5259 no esté en uso
- Cambiar el puerto en `launchSettings.json` si es necesario

### Error: "Database context disposed"
- Verificar que el contexto esté configurado correctamente en `Program.cs`
- Asegurar que se use `AddDbContext` con el scope correcto

### Error: "Mapeo de entidades fallido"
- Verificar que todos los adaptadores estén implementados correctamente
- Ejecutar `config.AssertConfigurationIsValid()` en desarrollo

## Historial de Versiones

### v2.0.0 - Actualización a .NET 10 (Marzo 2026)
**Cambios principales:**
- ✅ Actualización de .NET 9 → .NET 10 (LTS)
- ✅ Migración a paquetes modernos de API Versioning
  - `Microsoft.AspNetCore.Mvc.Versioning` → `Asp.Versioning.Mvc` 8.1.1
  - `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer` → `Asp.Versioning.Mvc.ApiExplorer` 8.1.1
- ✅ Actualización de MediatR a versión 14.1 con DI integrado
- ✅ Eliminación de paquetes deprecados
- ✅ Actualización de Entity Framework Core a 10.0.3
- ✅ Actualización de Swagger/OpenAPI a 10.0.3
- ✅ Actualización de Serilog a 10.0.0
- ✅ 81 tests unitarios e integración pasando (100%)

**Beneficios:**
- Soporte a largo plazo hasta noviembre 2028
- Mejor rendimiento y menor consumo de memoria
- Paquetes modernos con soporte activo
- Sin paquetes deprecados

### v1.0.0 - Versión Inicial (Original)
- Implementación inicial con .NET 9
- Arquitectura Hexagonal con CQRS
- 10 destinos turísticos de ejemplo
- Swagger/OpenAPI documentación

## Autor

**Lorelay Pricop Florescu**  
Graduada en Tecnologías Interactivas y Project Manager con experiencia en .NET, Python, Angular, Azure DevOps, IA y metodologías ágiles.

[LinkedIn](https://www.linkedin.com/in/lorelaypricop)  
Contacto: lorelaypricop@gmail.com

# Notas
> Algunas ideas relacionadas con validación, estilo y estructura se revisaron con el apoyo de herramientas de inteligencia artificial (IA), utilizadas para acelerar la documentación y validar casos límite.