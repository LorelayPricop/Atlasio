# Changelog

Todos los cambios notables en este proyecto serán documentados en este archivo.

El formato está basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/),
y este proyecto adhiere a [Semantic Versioning](https://semver.org/lang/es/).

## [2.1.0] - 2026-03-08

### Rebranding: HotelBediaX → Atlasio

#### Changed
- **Nombre de la aplicación**: HotelBediaX → Atlasio
- **Program.cs**: Mensaje de inicio actualizado a "Atlasio API"
- **Swagger**: Título de API actualizado a "Atlasio API"
- **Logging**: Propiedad Application actualizada a "Atlasio"
- **Archivos de log**: Renombrados de hotelbediax-*.log → atlasio-*.log
- **Documentación**: Todos los READMEs actualizados con nuevo nombre
- **README principal**: Actualizado con nombre Atlasio y .NET 10

#### Documentation
- README.md principal actualizado
- backend/README.md actualizado
- backend.Tests/README.md actualizado
- Todos los comentarios de código revisados

---

## [2.0.0] - 2026-03-08

### Actualización Mayor: Migración a .NET 10 (LTS)

#### Added
- Soporte para .NET 10 (LTS) con soporte hasta noviembre 2028
- Nuevas características de C# 13
- Mejoras de rendimiento de .NET 10
- Paquetes modernos de API Versioning (Asp.Versioning 8.x)
- Documentación actualizada con información de .NET 10
- Archivo CHANGELOG.md para seguimiento de versiones
- Historial de versiones en README.md

#### Changed
- **Framework**: .NET 9 → .NET 10 (LTS)
- **MediatR**: Migrado a configuración con DI integrado (v14.1.0)
- **API Versioning**: 
  - `Microsoft.AspNetCore.Mvc.Versioning` 5.1.0 → `Asp.Versioning.Mvc` 8.1.1
  - `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer` 5.1.0 → `Asp.Versioning.Mvc.ApiExplorer` 8.1.1
- **Entity Framework Core**: 9.0.8 → 10.0.3
- **Swagger/OpenAPI**: 9.0.8 → 10.0.3
- **Serilog**: 8.0.3 → 10.0.0
- Sintaxis de configuración de API Versioning a builder pattern moderno
- Namespaces de API Versioning: `Microsoft.AspNetCore.Mvc.Versioning` → `Asp.Versioning`

#### Removed
- `MediatR.Extensions.Microsoft.DependencyInjection` (funcionalidad integrada en MediatR 14.x)
- Paquetes deprecados de API Versioning

#### Fixed
- Configuración de Swagger simplificada para evitar dependencias de OpenAPI Models
- Agregado `Microsoft.EntityFrameworkCore.InMemory` al proyecto backend (faltaba)
- Agregado `Swashbuckle.AspNetCore.SwaggerGen` para soporte completo de Swagger

#### Testing
- ✅ Todos los 81 tests unitarios e integración pasando
- ✅ Sin cambios de comportamiento detectados
- ✅ Compatibilidad 100% con .NET 10
- Actualización de paquetes de testing:
  - `Microsoft.AspNetCore.Mvc.Testing` → 10.0.3
  - `Microsoft.EntityFrameworkCore.InMemory` → 10.0.3

#### Documentation
- README.md actualizado con información de .NET 10
- README.md de tests actualizado con estadísticas actuales
- Agregada sección de historial de versiones
- Documentación de paquetes NuGet actualizada
- Agregada sección de mejoras de .NET 10

---

## [1.0.0] - 2026-02-XX (Fecha Original)

### Versión Inicial

#### Added
- Arquitectura Hexagonal completa
- Implementación de CQRS con MediatR
- Repository Pattern y Unit of Work
- API REST con endpoints CRUD completos
- Versionado de API (v1)
- Documentación Swagger/OpenAPI
- Logging estructurado con Serilog
- 10 destinos turísticos de ejemplo
- Base de datos en memoria (Entity Framework Core)
- CORS configurado para Angular
- Manejo global de excepciones
- Tests unitarios completos (81 tests)
- Tests de integración end-to-end
- Adaptadores manuales para mapeo de entidades
- Filtrado y paginación de destinos
- Endpoints de soporte (countries, types)

#### Tech Stack (v1.0.0)
- .NET 9
- ASP.NET Core 9
- Entity Framework Core 9
- MediatR 11.1
- Serilog 8.0.3
- xUnit 2.6.1
- Moq 4.20.69
- FluentAssertions 6.12.0

---

## Tipos de Cambios

- **Added**: Para nuevas funcionalidades
- **Changed**: Para cambios en funcionalidades existentes
- **Deprecated**: Para funcionalidades que pronto serán removidas
- **Removed**: Para funcionalidades removidas
- **Fixed**: Para corrección de bugs
- **Security**: Para vulnerabilidades de seguridad

---

## Enlaces

- [Documentación de .NET 10](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10)
- [Asp.Versioning Documentation](https://github.com/dotnet/aspnet-api-versioning/wiki)
- [MediatR Documentation](https://github.com/jbogard/MediatR/wiki)
