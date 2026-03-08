# Tests Unitarios - Atlasio Backend

Este proyecto contiene los tests unitarios y de integración para el backend de Atlasio desarrollado en **.NET 10 (LTS)**.

## Estructura del Proyecto - Tests por Puertos/Contratos (Clean Architecture)

```
backend.Tests/
├── Ports/                # Tests de contratos/interfaces (Puertos)
│   ├── Repositories/     # Tests de contratos de repositorios
│   │   └── IDestinationRepositoryContractTests.cs
│   ├── Adapters/         # Tests de contratos de adaptadores
│   │   └── IDestinationAdapterContractTests.cs
│   └── Services/         # Tests de contratos de servicios
│       └── IUnitOfWorkContractTests.cs
├── UseCases/             # Tests de casos de uso (Application)
│   ├── Commands/         # Tests para command handlers
│   │   ├── CreateDestinationCommandHandlerTests.cs
│   │   ├── UpdateDestinationCommandHandlerTests.cs
│   │   └── DeleteDestinationCommandHandlerTests.cs
│   └── Queries/          # Tests para query handlers
│       ├── GetDestinationsQueryHandlerTests.cs
│       ├── GetDestinationByIdQueryHandlerTests.cs
│       ├── GetCountriesQueryHandlerTests.cs
│       └── GetDestinationTypesQueryHandlerTests.cs
├── Infrastructure/       # Tests de implementaciones concretas
│   ├── Repositories/     # Tests de implementación de repositorios
│   │   └── DestinationRepositoryTests.cs
│   ├── Services/         # Tests de implementación de servicios
│   │   └── DataSeedServiceTests.cs
│   └── UnitOfWork/       # Tests de implementación de Unit of Work
│       └── UnitOfWorkTests.cs
├── Domain/               # Tests para capa de dominio
│   ├── Entities/         # Tests para entidades
│   │   └── DestinationTests.cs
│   └── Enums/           # Tests para enumeraciones
│       └── DestinationTypeTests.cs
├── Presentation/         # Tests para capa de presentación
│   ├── Controllers/     # Tests para controladores
│   │   └── DestinationsControllerTests.cs
│   └── Middleware/     # Tests para middleware
│       └── GlobalExceptionMiddlewareTests.cs
├── Integration/          # Tests de integración (End-to-End)
│   └── DestinationsControllerIntegrationTests.cs
├── Helpers/              # Clases helper para tests
│   └── TestDataHelper.cs
└── README.md
```

### **Tests por Puertos/Contratos (Clean Architecture)**

#### **Ports Tests (Contratos/Interfaces)**
- **IDestinationRepositoryContractTests**: Tests de contrato para repositorio de destinos
- **IDestinationAdapterContractTests**: Tests de contrato para adaptadores de mapeo
- **IUnitOfWorkContractTests**: Tests de contrato para Unit of Work

#### **UseCases Tests (Casos de Uso)**
- **Command Handlers**: Tests para Create, Update, Delete destinations
- **Query Handlers**: Tests para GetDestinations, GetById, GetCountries, GetTypes

#### **Infrastructure Tests (Implementaciones Concretas)**
- **DestinationRepositoryTests**: Tests de implementación de repositorio
- **DataSeedServiceTests**: Tests de implementación de servicio de datos
- **UnitOfWorkTests**: Tests de implementación de Unit of Work

#### **Domain Tests (Entidades del Dominio)**
- **DestinationTests**: Tests para entidad del dominio
- **DestinationTypeTests**: Tests para enumeración de tipos

#### **Presentation Tests (Capa de Presentación)**
- **DestinationsControllerTests**: Tests unitarios para controladores
- **GlobalExceptionMiddlewareTests**: Tests para middleware de excepciones

#### **Integration Tests (End-to-End)**
- **DestinationsControllerIntegrationTests**: Tests end-to-end para API completa

### **Diferencias entre Ports e Infrastructure Tests**

#### **Ports Tests (Contratos/Interfaces)**
- **Propósito**: Verificar que cualquier implementación cumpla con el contrato
- **Independencia**: No dependen de implementaciones específicas
- **Reutilización**: Se pueden usar para probar múltiples implementaciones
- **Ejemplo**: `IDestinationRepositoryContractTests` verifica que cualquier repositorio cumpla el contrato

#### **Infrastructure Tests (Implementaciones Concretas)**
- **Propósito**: Verificar la implementación específica de un contrato
- **Dependencia**: Dependen de la implementación concreta (EF, InMemory, etc.)
- **Específicos**: Prueban detalles específicos de la implementación
- **Ejemplo**: `DestinationRepositoryTests` verifica la implementación con Entity Framework

### **Ventajas de la Estructura por Puertos/Contratos**

1. **Tests de Contrato**: Verifican que cualquier implementación cumpla con el contrato
2. **Independencia de Implementación**: Los tests de contrato no dependen de implementaciones específicas
3. **Facilita el Testing**: Se pueden probar múltiples implementaciones del mismo contrato
4. **Clean Architecture**: Refleja la separación entre puertos (interfaces) y adaptadores (implementaciones)
5. **Mantenibilidad**: Cambios en implementaciones no afectan tests de contrato
6. **Flexibilidad**: Permite cambiar implementaciones sin romper tests de contrato

## Tecnologías Utilizadas

- **xUnit 2.6.1**: Framework de testing moderno y extensible
- **Moq 4.20.69**: Framework de mocking para crear objetos simulados
- **FluentAssertions 6.12.0**: Librería de aserciones más legibles y expresivas
- **Microsoft.EntityFrameworkCore.InMemory 10.0.3**: Base de datos en memoria para tests (.NET 10)
- **Microsoft.AspNetCore.Mvc.Testing 10.0.3**: Testing de integración para APIs (.NET 10)
- **Microsoft.NET.Test.Sdk 17.8.0**: SDK de testing para .NET
- **coverlet.collector 6.0.0**: Herramienta de cobertura de código

## Tipos de Tests

### 1. Tests Unitarios

#### DestinationServiceTests
- Obtener destinos con y sin filtros
- Obtener destino por ID (válido e inválido)
- Crear nuevo destino
- Actualizar destino existente
- Eliminar destino
- Obtener lista de países
- Paginación y ordenamiento

#### DestinationsControllerTests
- Todos los endpoints GET, POST, PUT, DELETE
- Manejo de errores y excepciones
- Validación de ModelState
- Respuestas HTTP correctas (200, 201, 404, 500)

#### DataSeedServiceTests
- Poblar base de datos vacía
- No duplicar datos existentes
- Verificar destinos específicos
- Validar fechas y IDs únicos
- Cobertura de todos los tipos de destino

#### AdapterTests
- Mapeo de entidades a DTOs
- Mapeo de DTOs a entidades
- Mapeo de listas
- Mapeo con valores nulos y vacíos
- Validación de configuración

### 2. Tests de Integración

#### DestinationsControllerIntegrationTests
- Flujo completo de la API
- Base de datos real (en memoria)
- Filtros y paginación
- Validación de datos
- Respuestas HTTP completas

## Cómo Ejecutar los Tests

### Desde la línea de comandos

```bash
# Navegar al directorio del proyecto de tests
cd backend/backend.Tests

# Restaurar paquetes NuGet
dotnet restore

# Ejecutar todos los tests
dotnet test

# Ejecutar tests con cobertura de código
dotnet test --collect:"XPlat Code Coverage"

# Ejecutar tests con salida detallada
dotnet test --verbosity normal

# Ejecutar tests específicos
dotnet test --filter "ClassName=DestinationServiceTests"
```

### Desde Visual Studio

1. Abrir el proyecto en Visual Studio
2. Ir a **Test Explorer** (Test → Test Explorer)
3. Hacer clic en **Run All Tests** o ejecutar tests individuales

### Desde Visual Studio Code

1. Instalar la extensión **.NET Core Test Explorer**
2. Abrir el proyecto
3. Ejecutar tests desde el panel de Test Explorer

## Patrones de Testing Utilizados

### 1. Arrange-Act-Assert (AAA)
```csharp
[Fact]
public async Task GetDestination_WithValidId_ReturnsDestination()
{
    // Arrange
    var destination = TestDataHelper.CreateTestDestination();
    _context.Destinations.Add(destination);
    await _context.SaveChangesAsync();

    // Act
    var result = await _service.GetDestinationByIdAsync(destination.ID);

    // Assert
    result.Should().NotBeNull();
    result!.ID.Should().Be(destination.ID);
}
```

### 2. Mocking con Moq
```csharp
_mockService.Setup(s => s.GetDestinationByIdAsync(destinationId))
           .ReturnsAsync(expectedDestination);

// Mock del logger para servicios
_mockLogger = new Mock<ILogger<DestinationService>>();
_service = new DestinationService(_context, _mapper, _mockLogger.Object);
```

### 3. Base de Datos en Memoria
```csharp
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;
```

### 4. Aserciones con FluentAssertions
```csharp
result.Should().NotBeNull();
result.Items.Should().HaveCount(3);
result.TotalCount.Should().Be(3);
```

## Datos de Prueba

La clase `TestDataHelper` proporciona métodos para crear datos de prueba consistentes:

- `CreateTestDestination()`: Crea un destino individual
- `CreateTestDestinations()`: Crea una lista de destinos
- `CreateTestCreateDestinationDto()`: Crea DTO de creación
- `CreateTestUpdateDestinationDto()`: Crea DTO de actualización
- `CreateTestDestinationFilter()`: Crea filtro de búsqueda

## Mejores Prácticas Implementadas

1. **Aislamiento**: Cada test es independiente
2. **Limpieza**: Uso de `IDisposable` para limpiar recursos
3. **Nombres descriptivos**: Los nombres de los tests explican qué se está probando
4. **Datos de prueba consistentes**: Uso de helpers para crear datos
5. **Cobertura completa**: Tests para casos exitosos y de error
6. **Tests de integración**: Verificación del flujo completo
7. **Mocking de dependencias**: Uso de Moq para logger y servicios
8. **Logging en tests**: Verificación de que el logging funciona correctamente

## Estadísticas de Tests

**Estado Actual** (actualizado después de migración a .NET 10):
- ✅ **Total de Tests**: 81
- ✅ **Tests Pasando**: 81 (100%)
- ✅ **Tests Fallando**: 0
- ✅ **Tiempo de Ejecución**: ~4-9 segundos
- ✅ **Compatibilidad**: Totalmente compatible con .NET 10

**Distribución por Categoría**:
- Tests Unitarios de Dominio: ~10
- Tests Unitarios de Aplicación (CQRS): ~25
- Tests de Infraestructura: ~15
- Tests de Presentación (Controllers): ~20
- Tests de Integración (End-to-End): ~11

**Cobertura de Código** (estimada):
- Domain Layer: ~90%
- Application Layer: ~85%
- Infrastructure Layer: ~80%
- Presentation Layer: ~75%

## Notas de Migración a .NET 10

### Cambios Aplicados
- ✅ Actualización de `Microsoft.EntityFrameworkCore.InMemory` de 9.0.8 → 10.0.3
- ✅ Actualización de `Microsoft.AspNetCore.Mvc.Testing` de 9.0.8 → 10.0.3
- ✅ Todos los tests continúan pasando sin cambios de código
- ✅ Sin comportamientos inesperados detectados
- ✅ Compatibilidad 100% con .NET 10

### Validación Post-Migración
- ✅ Todos los 81 tests ejecutados exitosamente
- ✅ Sin cambios en comportamiento de assertions
- ✅ Sin cambios en comportamiento de API HTTP
- ✅ Tests de integración funcionando correctamente
- ✅ Base de datos en memoria funcionando sin cambios

## Troubleshooting

### Error: "Database already exists"
- Los tests usan nombres únicos de base de datos con `Guid.NewGuid()`
- Si persiste, limpiar la base de datos en memoria

### Error: "Mapeo de entidades fallido"
- Verificar que todos los adaptadores estén implementados correctamente
- Ejecutar `config.AssertConfigurationIsValid()` en los tests

### Tests lentos
- Usar base de datos en memoria en lugar de SQL Server
- Evitar operaciones de red en tests unitarios
- Usar mocks para dependencias externas

## Contribución

Al agregar nuevos tests:

1. Seguir el patrón AAA (Arrange-Act-Assert)
2. Usar nombres descriptivos para los tests
3. Agregar tests para casos de error
4. Mantener la cobertura de código alta
5. Documentar casos especiales o complejos

## Author

**Lorelay Pricop Florescu**  
Graduate in Interactive Technologies and Project Manager with experience in .NET, Python, Angular, Azure DevOps, AI, and Agile methodologies.

[LinkedIn](https://www.linkedin.com/in/lorelaypricop)  
Contact: lorelaypricop@gmail.com

# Notes
> Some ideas regarding validation, style, and structure were reviewed with the support of artificial intelligence (AI) tools, used to help accelerate documentation and validate edge case