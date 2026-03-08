# Backend-Driven Refactoring - Migration Notes

**Fecha**: 8 de marzo de 2026  
**Objetivo**: Migrar frontend de catálogos hardcodeados a backend-driven con caché centralizado

---

## 📋 Resumen Ejecutivo

Se refactorizó completamente el frontend de Angular para consumir catálogos dinámicos del backend en lugar de usar enums hardcodeados. Se implementó un servicio centralizado con caché en memoria para optimizar peticiones HTTP.

---

## 🗑️ Archivos Eliminados

### Enums Hardcodeados (Reemplazados por Backend)
```
✅ frontend/src/app/shared/enums/country.enum.ts
✅ frontend/src/app/shared/enums/city.enum.ts
✅ frontend/src/app/shared/enums/destination-type.enum.ts
```

**Razón**: Estos catálogos ahora se obtienen dinámicamente del backend a través de `CatalogService`.

### Componentes No Utilizados
```
✅ frontend/src/app/shared/modal/ (completo)
   - modal.component.ts
   - modal.component.html
   - modal.component.css
```

**Razón**: El flujo de crear/editar ahora usa páginas dedicadas (`/destinations/new`, `/destinations/:id/edit`) en lugar de modales.

---

## ✨ Archivos Nuevos

### 1. CatalogService - Servicio Centralizado
**Archivo**: `frontend/src/app/services/catalog.service.ts`

**Responsabilidades**:
- Gestión de catálogos: países, ciudades, tipos de destino
- Caché en memoria con signals de Angular
- Evitar peticiones HTTP duplicadas con `shareReplay`
- Manejo de errores centralizado

**Métodos principales**:
```typescript
getCountries(onlyActive?: boolean, forceRefresh?: boolean): Observable<CountryDto[]>
getCities(countryCode?: string, onlyActive?: boolean, forceRefresh?: boolean): Observable<CityDto[]>
getDestinationTypes(onlyActive?: boolean, forceRefresh?: boolean): Observable<DestinationTypeDto[]>
getCountryByCode(code: string): Observable<CountryDto | undefined>
getDestinationTypeById(id: number): Observable<DestinationTypeDto | undefined>
clearCache(): void
```

**Características de caché**:
- ✅ Países: cacheados globalmente
- ✅ Tipos de destino: cacheados globalmente
- ⚠️ Ciudades: cacheadas solo para listado completo, NO para filtros por país

### 2. Tests Unitarios
**Archivo**: `frontend/src/app/services/catalog.service.spec.ts`

**Cobertura**:
- ✅ Obtención de catálogos del backend
- ✅ Mecanismo de caché (evitar llamadas duplicadas)
- ✅ Forzar recarga con `forceRefresh`
- ✅ Manejo de errores (retornar array vacío)
- ✅ Búsqueda en caché antes de llamar backend
- ✅ Helpers de retrocompatibilidad

---

## 🔄 Archivos Modificados

### 1. create-destination-page.component.ts/html
**Cambios**:
- ❌ **Antes**: `COUNTRY_OPTIONS` (hardcoded), `DESTINATION_TYPE_OPTIONS` (enum hardcoded)
- ✅ **Ahora**: `CatalogService.getCountries()`, `CatalogService.getDestinationTypes()`
- ❌ **Antes**: `formModel.type: DestinationType | null`
- ✅ **Ahora**: `formModel.destinationTypeId: number | null`
- ❌ **Antes**: Template con `destinationTypeOptions` y `[ngValue]="option.value"`
- ✅ **Ahora**: Template con `destinationTypes()` del backend y `[ngValue]="type.id"`

**Impacto en UX**: Ninguno. El comportamiento visual es idéntico, pero ahora los catálogos son dinámicos.

### 2. edit-destination-page.component.ts/html
**Cambios**: Idénticos a `create-destination-page` (mismo patrón).

**Mejora adicional**: Eliminada dependencia de `getCountryDisplayName` helper local; ahora usa `CountryDto.name` directamente.

### 3. destination-detail-page.component.ts/html
**Cambios**:
- ❌ **Antes**: `getTypeLabel(type)` con mapping local
- ✅ **Ahora**: `dest.typeName` (viene directo del backend en `DestinationDto`)
- ❌ **Antes**: Helper `getCountryName` con inferencia por ciudad
- ✅ **Ahora**: Simplificado: solo muestra `countryCode` (opcional: cargar nombre del catálogo si es necesario)

**Impacto en UX**: Tipo de destino ahora muestra el nombre exacto del catálogo backend en lugar de un mapeo hardcoded.

### 4. destinations-page.component.ts/html
**Refactor Mayor** - Eliminaciones:
- ❌ Sorting client-side (`onSort`, `getSortedDestinations`, `sortState`)
- ❌ Modal inline para crear/editar
- ❌ Formulario de creación/edición en lista
- ❌ Conversión index ↔ enum (`getDestinationTypeFromIndex`, `getIndexFromDestinationType`)
- ❌ `loadDestinationTypes()` usando endpoint legacy `/api/v1/types`
- ❌ Helpers `getTypeLabel` con mapping hardcoded
- ❌ Importaciones de `COUNTRY_OPTIONS`, `CITY_OPTIONS`, `DestinationType`, `DestinationCategory`

**Refactor Mayor** - Adiciones:
- ✅ `CatalogService` injection
- ✅ Catálogos `countries: CountryDto[]`, `destinationTypes: DestinationTypeDto[]`
- ✅ Filtros backend: `destinationTypeId: number | null` en lugar de string/enum
- ✅ Navegación a páginas dedicadas (`/destinations/new`, `/destinations/:id/edit`)
- ✅ `getCountryDisplayName` busca en `CountryDto[]` en lugar de helper hardcoded
- ✅ `getTypeIcon` y `getTypeClass` basados en `typeCode` (string) del catálogo backend
- ✅ Eliminado componente `ModalComponent` del import

**Backend query params actualizados**:
```typescript
// ❌ Antes:
apiService.destinationsGET(
  search,
  countryCode,
  type as DestinationType,  // ← enum
  page,
  pageSize
)

// ✅ Ahora:
apiService.destinationsGET(
  search,
  countryCode,
  destinationTypeId,  // ← number (ID del tipo de destino)
  page,
  pageSize
)
```

**Impacto en UX**:
- ✅ **Mejor**: Create/Edit ahora tienen páginas dedicadas con mejor UX
- ✅ **Mejor**: Catálogos siempre actualizados desde backend
- ⚠️ **Cambio**: No hay sorting client-side (el backend debería implementar `sortBy` y `sortDir` si se requiere)

---

## 🔌 Contratos Backend Requeridos

### API Catalog Endpoints

#### 1. GET `/api/v1/Catalog/countries`
**Query params**:
- `onlyActive?: boolean` (default: true)

**Response**: `CountryDto[]`
```typescript
{
  code: string;        // ISO 3166-1 alpha-3
  name: string;        // Nombre completo del país
  region?: string;     // Región geográfica
  isActive?: boolean;  // Si está activo
}
```

#### 2. GET `/api/v1/Catalog/countries/{code}`
**Path params**:
- `code: string` (ISO 3166-1 alpha-3)

**Response**: `CountryDto`

#### 3. GET `/api/v1/Catalog/cities`
**Query params**:
- `countryCode?: string` (filtrar por país)
- `onlyActive?: boolean` (default: true)

**Response**: `CityDto[]`
```typescript
{
  id?: number;
  countryCode: string;
  name: string;
  isActive?: boolean;
}
```

#### 4. GET `/api/v1/Catalog/destination-types`
**Query params**:
- `onlyActive?: boolean` (default: true)

**Response**: `DestinationTypeDto[]`
```typescript
{
  id?: number;
  code: string;           // Código único (ej: 'BEACH', 'MOUNTAIN')
  name: string;           // Nombre para mostrar (ej: 'Beach', 'Mountain')
  icon?: string;          // Identificador de icono o clase CSS
  displayOrder?: number;  // Orden de visualización
  isActive?: boolean;     // Si está activo
}
```

#### 5. GET `/api/v1/Catalog/destination-types/{id}`
**Path params**:
- `id: number`

**Response**: `DestinationTypeDto`

### Destinations Endpoints

#### GET `/api/v1/Destinations`
**Query params actualizados**:
- `searchTerm?: string`
- `countryCode?: string`
- `destinationTypeId?: number` ← **Cambiado de enum a ID**
- `page?: number`
- `pageSize?: number`

**Response**: `DestinationDtoPagedResultDto`

#### POST `/api/v1/Destinations`
**Body**: `CreateDestinationDto`
```typescript
{
  name: string;
  description: string;
  longDescription?: string;
  imageUrl?: string;
  countryCode: string;
  destinationTypeId: number;  // ← ID del tipo de destino
}
```

#### PUT `/api/v1/Destinations/{id}`
**Body**: `UpdateDestinationDto`
```typescript
{
  name: string;
  description: string;
  longDescription?: string;
  imageUrl?: string;
  countryCode: string;
  destinationTypeId: number;  // ← ID del tipo de destino
}
```

#### GET `/api/v1/Destinations/{id}`
**Response**: `DestinationDto`
```typescript
{
  id: number;
  name: string;
  description: string;
  longDescription?: string;
  countryCode: string;
  destinationTypeId: number;
  typeName?: string;  // ← Nombre del tipo (para mostrar en UI)
  lastModif: Date;
  imageUrl?: string;
  totalBookings?: number;
  averageRating?: number;
  reviewCount?: number;
  status?: string;
  createdBy?: string;
  createdDate: Date;
}
```

**IMPORTANTE**: `typeName` debe ser poblado por el backend (join con catálogo de tipos).

---

## 🧪 Cómo Ejecutar Tests

```bash
# Ejecutar tests del CatalogService
cd frontend
npm test -- --include='**/catalog.service.spec.ts'

# Ejecutar todos los tests
npm test

# Ejecutar con cobertura
npm test -- --code-coverage
```

---

## 🚀 Cómo Desplegar

### 1. Regenerar API Client (si hay cambios backend)
```bash
cd frontend
npm run generate-api
```

### 2. Build para producción
```bash
npm run build
```

### 3. Validar que catálogos backend estén poblados
Antes de desplegar, verificar que el backend tenga:
- ✅ Tabla `Countries` con registros activos
- ✅ Tabla `Cities` con registros activos (si aplica)
- ✅ Tabla `DestinationTypes` con registros activos
- ✅ Seed data cargada correctamente

---

## ⚠️ Breaking Changes

### Para Desarrolladores Frontend

1. **Ya NO usar enums hardcodeados**:
   ```typescript
   // ❌ NO hacer esto:
   import { COUNTRY_OPTIONS } from '../shared/enums/country.enum';
   
   // ✅ Hacer esto:
   import { CatalogService } from '../services/catalog.service';
   catalogService.getCountries().subscribe(...);
   ```

2. **Cambio en modelo de formularios**:
   ```typescript
   // ❌ Antes:
   formModel: { type: DestinationType | null }
   
   // ✅ Ahora:
   formModel: { destinationTypeId: number | null }
   ```

3. **Eliminado ModalComponent**:
   - No importar `ModalComponent` en nuevos componentes
   - Usar páginas dedicadas para flujos CRUD

### Para Backend

1. **Endpoints de catálogo son obligatorios**:
   - `/api/v1/Catalog/countries`
   - `/api/v1/Catalog/destination-types`
   - `/api/v1/Catalog/cities` (opcional pero recomendado)

2. **DestinationDto debe incluir `typeName`**:
   - Poblar desde join con tabla `DestinationTypes`
   - NO enviar solo el ID sin el nombre

3. **CreateDestinationDto y UpdateDestinationDto**:
   - Cambiar de `type: DestinationType` → `destinationTypeId: number`

---

## 📊 Mejoras de Performance

### Antes (Hardcoded Enums)
- 0 peticiones HTTP para catálogos
- Catálogos estáticos, requieren rebuild para actualizar

### Ahora (Backend-Driven + Caché)
- **Primera carga**: 2-3 peticiones HTTP concurrentes (países, tipos, ciudades)
- **Navegación subsecuente**: 0 peticiones adicionales (caché en memoria)
- **Actualización**: Backend puede actualizar catálogos sin rebuild del frontend
- **Optimización**: `shareReplay(1)` evita llamadas duplicadas simultáneas

### Tamaño del Bundle
- **Antes**: ~351 KB (initial total)
- **Ahora**: ~351 KB (sin cambio significativo, código más mantenible)

---

## 🔮 Futuras Mejoras Recomendadas

1. **Backend Sorting**:
   - Implementar query params `sortBy` y `sortDir` en `GET /api/v1/Destinations`
   - Eliminar sorting client-side completamente

2. **Paginación Catálogos**:
   - Si catálogos crecen mucho, implementar paginación en endpoints de catálogo
   - Optimizar caché con estrategia LRU si es necesario

3. **Lazy Loading de Catálogos**:
   - Actualmente todos los catálogos se cargan en `ngOnInit`
   - Considerar cargar solo cuando usuario interactúa con filtros

4. **Persistencia de Caché**:
   - Actualmente caché es solo en memoria (se pierde al refrescar)
   - Considerar `localStorage` para persistir catálogos entre sesiones

5. **Refresh Automático**:
   - Implementar mecanismo de refresh periódico (ej: cada 30 min)
   - O usar WebSockets/SignalR para notificaciones de cambios en catálogos

---

## 📝 Checklist de Verificación Post-Migración

- [x] Build exitoso sin errores TypeScript
- [x] Tests unitarios de `CatalogService` pasando
- [x] Archivos deprecados eliminados
- [x] API client regenerado con últimos cambios backend
- [ ] **Validar manualmente**:
  - [ ] Crear destino funciona con nuevos catálogos
  - [ ] Editar destino funciona con nuevos catálogos
  - [ ] Detalle de destino muestra `typeName` correctamente
  - [ ] Filtros en lista de destinos funcionan con IDs
  - [ ] Caché funciona (no hay peticiones duplicadas)
- [ ] **Backend**:
  - [ ] Endpoints de catálogo implementados
  - [ ] Seed data cargada
  - [ ] `DestinationDto.typeName` poblado
  - [ ] DTOs actualizados (`destinationTypeId` en lugar de enum)

---

## 🆘 Troubleshooting

### Error: "Cannot find module 'country.enum'"
**Solución**: El archivo fue eliminado. Usar `CatalogService.getCountries()` en su lugar.

### Error: "Property 'type' does not exist on type 'CreateDestinationDto'"
**Solución**: El campo cambió a `destinationTypeId`. Actualizar formulario y DTOs.

### Catálogos vacíos en frontend
**Solución**:
1. Verificar que backend esté corriendo
2. Verificar que endpoints `/api/v1/Catalog/*` respondan correctamente
3. Verificar seed data en base de datos
4. Revisar consola del navegador para errores HTTP

### Caché no funciona correctamente
**Solución**: Llamar `catalogService.clearCache()` para resetear y verificar que peticiones subsecuentes usen caché.

---

**✅ Migración completada exitosamente**
