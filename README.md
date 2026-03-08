# Atlasio - Portal de Gestión de Destinos Turísticos

## Descripción del Proyecto

**Atlasio** es un portal completo de gestión de destinos turísticos desarrollado para FDSA. La aplicación implementa **Arquitectura Hexagonal** (Ports and Adapters) con patrones CQRS, Repository y Unit of Work, permitiendo a los usuarios gestionar destinos turísticos con operaciones CRUD completas, filtrado avanzado, documentación automática con Swagger y una arquitectura optimizada y mantenible.

### Objetivos de la Prueba Técnica

- **Backend**: API REST con .NET 10 (LTS)
- **Frontend**: SPA con Angular (módulo Destinations implementado)
- **Base de Datos**: Mock database para demostración
- **Funcionalidades**: CRUD completo + filtrado + paginación
- **Rendimiento**: Optimizado para manejar 200k+ registros
- **Documentación**: Swagger/OpenAPI integrado
- **Testing**: Suite completa de pruebas unitarias e integración
- **Logging**: Logging estructurado con Serilog
- **Arquitectura**: Arquitectura Hexagonal (Ports and Adapters)
- **Patrones**: CQRS + Repository Pattern + Unit of Work + MediatR
- **API Versioning**: Infraestructura preparada para versionado futuro

## Arquitectura del Proyecto

### Estructura General

```
Atlasio/
├── backend/                 # API REST con .NET 10 (LTS)
│   ├── backend/            # Proyecto principal con Arquitectura Hexagonal
│   │   ├── Domain/         # Entidades, enums e interfaces
│   │   ├── Application/    # Commands, Queries, DTOs y Mapping
│   │   ├── Infrastructure/ # Repositorios, contexto EF y servicios
│   │   ├── Presentation/   # Controladores y middleware
│   │   └── Program.cs      # Configuración y startup
│   ├── backend.Tests/      # Suite completa de pruebas
│   │   ├── Domain/         # Tests de entidades
│   │   ├── Application/    # Tests de CQRS
│   │   ├── Infrastructure/ # Tests de repositorios
│   │   ├── Presentation/   # Tests de controladores
│   │   └── Integration/    # Tests end-to-end
│   └── backend.sln         # Solución Visual Studio
├── frontend/               # Aplicación Angular 21
│   ├── src/               # Código fuente
│   │   ├── app/           # Módulos y componentes
│   │   │   ├── destinations/    # Módulo de destinos
│   │   │   ├── shared/         # Componentes compartidos
│   │   │   ├── services/       # Servicios Angular
│   │   │   └── interceptors/   # Interceptores HTTP
│   │   ├── index.html     # Página principal
│   │   └── main.ts        # Bootstrap de la app
│   ├── public/            # Assets estáticos
│   ├── package.json       # Dependencias Node.js
│   └── angular.json       # Configuración Angular
└── README.md               # Este archivo
```

### Arquitectura Hexagonal (Ports and Adapters) - Clean Architecture

El backend implementa **Arquitectura Hexagonal** con principios de **Clean Architecture** que separa el código en capas bien definidas:

- **Domain Layer**: Entidades, enums e interfaces del dominio (completamente independiente de frameworks y otras capas)
- **Application Layer**: Casos de uso, CQRS (Commands/Queries), DTOs, adaptadores y mapeos
- **Infrastructure Layer**: Repositorios, contexto de datos, servicios externos
- **Presentation Layer**: Controladores de API, middleware y DTOs de presentación

**Principios de Clean Architecture implementados**:
- **Independencia de frameworks**: El dominio no depende de ninguna librería externa
- **Testabilidad**: Cada capa puede ser probada independientemente
- **Independencia de UI**: La lógica de negocio no depende de la interfaz
- **Independencia de base de datos**: El dominio no conoce detalles de persistencia
- **Independencia de agentes externos**: La lógica de negocio no depende de servicios externos

**Beneficios**: Testabilidad, independencia de frameworks, flexibilidad, mantenibilidad, escalabilidad y adherencia a principios SOLID.

## Funcionalidades Principales

### Gestión de Destinos Turísticos
- **Crear** nuevos destinos turísticos
- **Leer** y visualizar destinos existentes
- **Actualizar** información de destinos
- **Eliminar** destinos no deseados
- **Filtrar** por país, tipo de destino o texto
- **Paginación** para manejar grandes volúmenes de datos

### Tipos de Destinos Soportados
- **Beach** - Destinos de playa y costa
- **Mountain** - Destinos de montaña
- **City** - Destinos urbanos
- **Cultural** - Patrimonio cultural e histórico
- **Adventure** - Actividades de aventura
- **Relax** - Destinos de relajación

## Datos de Ejemplo Incluidos

El sistema incluye **10 destinos turísticos reales** con datos completos:

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

## Cómo Ejecutar el Proyecto

### Prerrequisitos

#### Backend
- **.NET 10 SDK** instalado
- **Visual Studio 2022** o **VS Code** (recomendado)

#### Frontend
- **Node.js** (versión 20.19 o superior recomendada para Angular 21)
- **npm** (incluido con Node.js)
- **Angular 21** (versión específica del proyecto)
- **Angular CLI**

### Ejecutar el Backend

```bash
# Navegar al directorio del backend
cd backend/backend

# Restaurar dependencias
dotnet restore

# Compilar el proyecto
dotnet build

# Ejecutar la aplicación
dotnet run
```

### Ejecutar el Frontend

```bash
# Navegar al directorio del frontend
cd frontend

# Instalar dependencias
npm install

# Ejecutar la aplicación
npm start
```

### Acceso a la Aplicación

- **API REST v1.0**: `http://localhost:5259/api/v1.0/destinations`
- **Swagger UI**: `http://localhost:5259/swagger` (solo en desarrollo)
- **Frontend**: `http://localhost:4200`

## Testing

### Ejecutar las Pruebas

```bash
# Ejecutar todas las pruebas
cd backend/backend.Tests
dotnet test
```

## Características Técnicas Implementadas

### **Arquitectura Hexagonal (Ports and Adapters) - Clean Architecture**
- **Domain Layer** - Lógica de negocio pura e independiente de frameworks y otras capas
- **Application Layer** - Casos de uso con CQRS (Commands/Queries), adaptadores y DTOs
- **Infrastructure Layer** - Implementaciones concretas de repositorios y servicios
- **Presentation Layer** - Controladores de API y middleware
- **Repository Pattern** - Abstracción del acceso a datos con interfaces del dominio
- **Unit of Work** - Coordinación de transacciones complejas
- **MediatR** - Desacoplamiento entre controladores y lógica de negocio
- **Handlers especializados** - Cada operación tiene su handler específico
- **Adaptadores** - Conversión entre interfaces del dominio y DTOs de aplicación
- **Interfaces del dominio** - IPagedResult, IFilterCriteria para mantener independencia
- **API Versioning** - Infraestructura preparada para versionado futuro

### **Mejoras de Clean Architecture Implementadas**
- **Eliminación de dependencias incorrectas**: El dominio ya no depende de DTOs de Application
- **Interfaces del dominio**: IPagedResult, IFilterCriteria e IUnitOfWork para mantener independencia
- **Adaptadores**: DestinationFilterAdapter, PagedResultAdapter y EfDbContextTransaction para conversión entre capas
- **Separación de responsabilidades**: Cada capa tiene responsabilidades bien definidas
- **Testabilidad mejorada**: Los tests pueden usar interfaces del dominio sin acoplamiento
- **Application independiente de Infrastructure**: Los handlers usan interfaces del dominio, no implementaciones concretas

### **Logging Estructurado**
- **Serilog** configurado con múltiples sinks (Console, File)
- **Logging eficiente** - solo operaciones críticas
- **Logs estructurados** con propiedades JSON para búsqueda eficiente
- **Rotación automática** de archivos de log
- **Contexto completo** en errores con RequestId único
- **Configuración por ambiente** (Development vs Production)

## Documentación Detallada

Para información más específica sobre cada componente del proyecto, consulta los READMEs correspondientes:

- **[Backend API](./backend/backend/README.md)** - Documentación completa del backend con Arquitectura Hexagonal, endpoints, configuración y troubleshooting
- **[Frontend](./frontend/src/app/README.md)** - Documentación completa del frontend Angular con arquitectura, componentes y funcionalidades
- **[Testing](./backend/backend.Tests/README.md)** - Documentación completa de las pruebas unitarias e integración, cobertura y mejores prácticas

## Autor

**Lorelay Pricop Florescu**  
Graduada en Tecnologías Interactivas y Project Manager con experiencia en .NET, Python, Angular, Azure DevOps, IA y metodologías ágiles.

[LinkedIn](https://www.linkedin.com/in/lorelaypricop)  
Contacto: lorelaypricop@gmail.com

## Notas
> Algunas ideas relacionadas con validación, estilo y estructura se revisaron con el apoyo de herramientas de inteligencia artificial (IA), utilizadas para acelerar la documentación y validar casos límite.
