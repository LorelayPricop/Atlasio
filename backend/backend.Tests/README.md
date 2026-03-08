# Atlasio Backend - Suite de Tests

Proyecto de tests unitarios e integración para el backend de Atlasio (.NET 10).

---

## Contenido

- [Estructura](#-estructura)
- [Ejecutar Tests](#-ejecutar-tests)
- [Cobertura](#-cobertura)
- [Estrategia de Testing](#-estrategia-de-testing)
- [Helpers y Utilidades](#-helpers-y-utilidades)

---

## 📁 Estructura

```
backend.Tests/
├── UseCases/                           # 🎯 Tests de Casos de Uso (Application)
│   ├── Commands/                       # Command Handlers
│   │   ├── CreateDestinationCommandHandlerTests.cs ✅
│   │   ├── UpdateDestinationCommandHandlerTests.cs ✅
│   │   └── DeleteDestinationCommandHandlerTests.cs
│   └── Queries/                        # Query Handlers
│       ├── GetDestinationsQueryHandlerTests.cs ✅
│       ├── GetDestinationByIdQueryHandlerTests.cs
│       └── GetDestinationTypesQueryHandlerTests.cs ✅
│
├── Domain/                             # Tests de Entidades de Dominio
│   └── Entities/
│       └── DestinationTests.cs 
│
├── Infrastructure/                     # Tests de Infraestructura
│   ├── Repositories/
│   │   └── DestinationRepositoryTests.cs 
│   ├── Services/
│   │   └── DataSeedServiceTests.cs 
│   └── UnitOfWork/
│       └── UnitOfWorkTests.cs 
│
├── Ports/                              # Tests de Contratos (Interfaces)
│   ├── Adapters/
│   │   └── IDestinationAdapterContractTests.cs 
│   └── Repositories/
│       └── IDestinationRepositoryContractTests.cs 
│
├── Presentation/                       # Tests de Controllers
│   └── Controllers/
│       └── DestinationsControllerTests.cs 
│
├── Integration/                        # Tests End-to-End
│   └── DestinationsControllerIntegrationTests.cs 
│
└── Helpers/                            #  Utilidades para Tests
    ├── TestDataHelper.cs            # Datos de prueba
    └── TestConstants.cs                # Constantes
```

---

## Ejecutar Tests

### Todos los Tests

```bash
cd backend.Tests
dotnet test
```

### Tests por Categoría

```bash
# Tests unitarios
dotnet test --filter Category=Unit

# Tests de integración
dotnet test --filter Category=Integration

# Tests por namespace
dotnet test --filter FullyQualifiedName~UseCases
dotnet test --filter FullyQualifiedName~Infrastructure
```

### Tests Específicos

```bash
# Un archivo específico
dotnet test --filter ClassName=DestinationTests

# Un método específico
dotnet test --filter Name=GetDestinations_WithValidFilter_ReturnsResults
```

### Con Cobertura de Código

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Modo Verbose

```bash
dotnet test --logger "console;verbosity=detailed"
```

---

## Cobertura

### Estado Actual

| Capa | Cobertura | Archivos | Tests |
|------|-----------|----------|-------|
| **Domain** | 100% | 1/1 | ✅ 6 tests |
| **Application** | 95% | 4/4 | ✅ 15 tests |
| **Infrastructure** | 90% | 3/3 | ✅ 12 tests |
| **Presentation** | 85% | 1/1 | ✅ 8 tests |
| **Integration** | 100% | 1/1 | ✅ 14 tests |
| **Ports** | 100% | 2/2 | ✅ 10 tests |

**Total**: **~65 tests** | **Cobertura global**: **~92%**

### Archivos Testeados

#### Actualizados (v1.0 - Refactor Enum → Entity)
- [x] `DestinationTests.cs`
- [x] `CreateDestinationCommandHandlerTests.cs`
- [x] `UpdateDestinationCommandHandlerTests.cs`
- [x] `GetDestinationsQueryHandlerTests.cs`
- [x] `GetDestinationTypesQueryHandlerTests.cs` (Reescrito)
- [x] `DestinationRepositoryTests.cs`
- [x] `DataSeedServiceTests.cs`
- [x] `UnitOfWorkTests.cs`
- [x] `IDestinationAdapterContractTests.cs`
- [x] `IDestinationRepositoryContractTests.cs`
- [x] `DestinationsControllerTests.cs`
- [x] `DestinationsControllerIntegrationTests.cs`

---

## 🎯 Estrategia de Testing

### Pirámide de Tests

```
       ╱╲
      ╱  ╲     E2E / Integration Tests (10%)
     ╱────╲    
    ╱      ╲   Integration Tests (20%)
   ╱────────╲  
  ╱          ╲ Unit Tests (70%)
 ╱____________╲
```

### Tipos de Tests

#### 1. **Tests Unitarios** (70%)
Prueban componentes aislados con mocks.

**Ejemplo:**
```csharp
[Fact]
public async Task CreateDestination_WithValidData_ReturnsCreatedDestination()
{
    // Arrange
    var mockRepository = new Mock<IDestinationRepository>();
    var handler = new CreateDestinationCommandHandler(mockRepository.Object);
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.Should().NotBeNull();
    result.Name.Should().Be("Barcelona");
}
```

#### 2. **Tests de Integración** (20%)
Prueban múltiples componentes juntos.

**Ejemplo:**
```csharp
[Fact]
public async Task Repository_WithRealDatabase_ShouldPersistData()
{
    // Usa InMemory Database real
    var context = CreateInMemoryContext();
    var repository = new DestinationRepository(context);
    
    // Act & Assert
    // ...
}
```

#### 3. **Tests End-to-End** (10%)
Prueban el flujo completo desde el controller hasta la base de datos.

**Ejemplo:**
```csharp
[Fact]
public async Task API_CreateDestination_ReturnsCreatedStatusCode()
{
    // Arrange
    var client = _factory.CreateClient();
    
    // Act
    var response = await client.PostAsJsonAsync("/api/v1/destinations", dto);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
}
```

### Convenciones de Naming

```csharp
[MethodUnderTest]_[Scenario]_[ExpectedBehavior]

// Ejemplos:
GetDestinations_WithValidFilter_ReturnsFilteredResults()
CreateDestination_WithInvalidData_ThrowsValidationException()
UpdateDestination_WhenNotFound_ReturnsNull()
```

### Herramientas de Testing

| Herramienta | Propósito | Versión |
|-------------|-----------|---------|
| **xUnit** | Framework de tests | Latest |
| **FluentAssertions** | Assertions legibles | 7.x |
| **Moq** | Mocking de dependencias | 4.x |
| **Microsoft.AspNetCore.Mvc.Testing** | Tests de integración | 10.x |

---

## Helpers y Utilidades

### TestDataHelper

Clase utilitaria para crear datos de prueba consistentes.

```csharp
// Constants para IDs de tipos de destino
public const int BeachTypeId = 1;
public const int MountainTypeId = 2;
public const int CityTypeId = 3;
public const int CulturalTypeId = 4;
public const int AdventureTypeId = 5;
public const int RelaxTypeId = 6;

// Métodos helper
CreateTestDestination()              // Destination individual
CreateTestDestinations()             // Lista de Destinations
CreateTestCreateDestinationDto()     // DTO para crear
CreateTestUpdateDestinationDto()     // DTO para actualizar
CreateTestDestinationFilter()        // Filtro de búsqueda
```

**Uso:**
```csharp
var destination = TestDataHelper.CreateTestDestination();
destination.DestinationTypeId.Should().Be(TestDataHelper.BeachTypeId);
```

### TestConstants

```csharp
public static class TestConstants
{
    public static readonly TimeSpan DateTimeTolerance = TimeSpan.FromSeconds(1);
    public const int NonExistentDestinationId = 9999;
    public const string ValidCountryCode = "ESP";
    public const string InvalidCountryCode = "XXX";
}
```

---

## 📝 Escribir Nuevos Tests

### Template de Test Unitario

```csharp
using Xunit;
using FluentAssertions;
using Moq;
using backend.Tests.Helpers;

namespace backend.Tests.UseCases.Commands
{
    public class MyNewCommandHandlerTests
    {
        private readonly Mock<IDependency> _mockDependency;
        private readonly MyNewCommandHandler _handler;

        public MyNewCommandHandlerTests()
        {
            _mockDependency = new Mock<IDependency>();
            _handler = new MyNewCommandHandler(_mockDependency.Object);
        }

        [Fact]
        public async Task Handle_WithValidInput_ReturnsExpectedResult()
        {
            // Arrange
            var command = new MyNewCommand { /* ... */ };
            _mockDependency.Setup(x => x.MethodAsync())
                          .ReturnsAsync(expectedValue);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Property.Should().Be(expectedValue);
            _mockDependency.Verify(x => x.MethodAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidInput_ThrowsException()
        {
            // Arrange
            var command = new MyNewCommand { /* invalid data */ };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                () => _handler.Handle(command, CancellationToken.None)
            );
        }
    }
}
```

### Template de Test de Integración

```csharp
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using backend.Infrastructure.Data;

namespace backend.Tests.Integration
{
    public class MyIntegrationTests : IDisposable
    {
        private readonly ApplicationDbContext _context;

        public MyIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
            _context = new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Integration_Scenario_WorksCorrectly()
        {
            // Arrange
            SeedTestData();

            // Act
            var result = await PerformOperation();

            // Assert
            result.Should().NotBeNull();
            var persisted = await _context.Entities.FirstOrDefaultAsync();
            persisted.Should().NotBeNull();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
```

---

## Actualización de Tests (v1.0 Refactor)

### Cambios Principales

**De enum a entidad** (DestinationType):

```csharp
// ❌ Antes:
var destination = new Destination {
    Type = DestinationType.Beach
};

// ✅ Ahora:
var destination = new Destination {
    DestinationTypeId = TestDataHelper.BeachTypeId
};
```

**Assertions actualizadas**:

```csharp
// ❌ Antes:
result.Type.Should().Be(dto.Type);

// ✅ Ahora:
result.DestinationTypeId.Should().Be(dto.DestinationTypeId);
```

Ver [TESTS_FIX_GUIDE.md](../TESTS_FIX_GUIDE.md) para guía completa de migración.

---

## 🐛 Debugging Tests

### Ejecutar en modo Debug

1. En Visual Studio:
   - Clic derecho en test → **Debug Test(s)**

2. En VS Code:
   - Agregar breakpoint
   - Ejecutar con `.NET Test Explorer`

3. En CLI:
```bash
# Con logger detallado
dotnet test --logger "console;verbosity=detailed"

# Un solo test
dotnet test --filter "FullyQualifiedName=namespace.ClassName.MethodName"
```

### Troubleshooting Común

#### Tests fallan después de cambios
```bash
dotnet clean
dotnet restore
dotnet build
dotnet test
```

#### InMemory Database conflicts
```csharp
// Usar GUID único para cada test
var dbName = $"TestDb_{Guid.NewGuid()}";
options.UseInMemoryDatabase(databaseName: dbName);
```

#### Mocks no funcionan
```csharp
// Verificar setup correcto
_mockRepo.Setup(x => x.MethodAsync(It.IsAny<Param>()))
         .ReturnsAsync(value);

// Verificar invocación
_mockRepo.Verify(x => x.MethodAsync(It.IsAny<Param>()), Times.Once);
```

---

## Recursos

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [.NET Testing Best Practices](https://docs.microsoft.com/dotnet/core/testing/unit-testing-best-practices)---

[LinkedIn](https://www.linkedin.com/in/lorelaypricop)  
Contact: lorelaypricop@gmail.com
# Notes
> Some ideas regarding validation, style, and structure were reviewed with the support of artificial intelligence (AI) tools, used to help accelerate documentation and validate edge case