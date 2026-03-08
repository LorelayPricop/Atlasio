# Atlasio - Portal de Gestión de Destinos Turísticos

## Descripción del Proyecto

**Atlasio** es un portal completo de gestión de destinos turísticos desarrollado con **.NET 10 (LTS)**. La aplicación implementa **Arquitectura Hexagonal** (Ports and Adapters) con patrones CQRS, Repository y Unit of Work, permitiendo a los usuarios gestionar destinos turísticos con operaciones CRUD completas, filtrado avanzado, documentación automática con Swagger y una arquitectura optimizada y mantenible.

### Objetivos del Proyecto

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
- **API Versioning**: Versionado moderno con Asp.Versioning 8.x

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
├── frontend/               # Aplicación Angular 18
│   ├── src/               # Código fuente
│   │   ├── app/           # Módulos y componentes
│   │   │   ├── destinations/    # Módulo de destinos
│   │   │   ├── shared/         # Componentes compartidos
│   │   │   ├── services/       # Servicios Angular
│   │   │   └── interceptors/   # Interceptores HTTP
│   │   ├── index.html     # Página principal
│   │   └── main.ts        # Bootstrap de la app
│   ├── public/            # Assets estáticos
// ...existing code...
```