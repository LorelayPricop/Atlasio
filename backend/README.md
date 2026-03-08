# Backend Atlasio

Guia rapida del backend de Atlasio con API REST en .NET 10 (LTS), arquitectura hexagonal y pruebas automatizadas.

## Estructura

```
backend/
├── backend.sln
├── backend/         # API principal (.NET 10)
└── backend.Tests/   # Tests unitarios e integracion
```

## Requisitos

- .NET SDK 10
- Visual Studio 2022 o VS Code

## Ejecutar API

```bash
cd backend/backend
dotnet restore
dotnet run
```

## Ejecutar tests

```bash
cd backend/backend.Tests
dotnet test
```

## Endpoints locales

- API: `http://localhost:5259/api/v1/destinations`
- Swagger: `http://localhost:5259/swagger`

## Documentacion detallada

- API completa: `backend/backend/README.md`
- Suite de tests: `backend/backend.Tests/README.md`
