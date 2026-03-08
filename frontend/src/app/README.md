# Frontend - Atlasio

## Arquitectura Modular Escalable

Este frontend está organizado siguiendo las mejores prácticas de Angular con una arquitectura modular que facilita el mantenimiento y la escalabilidad. Utiliza Angular 21 con componentes standalone y TypeScript para una experiencia de desarrollo moderna y eficiente.

## Características Principales

### **Gestión de Destinos Turísticos**
- **CRUD Completo**: Crear, leer, actualizar y eliminar destinos
- **Filtros Avanzados**: Búsqueda por nombre, país y tipo de destino
- **Paginación**: Navegación eficiente por grandes conjuntos de datos
- **Ordenamiento**: Ordenar por cualquier columna (ID, nombre, descripción, país, tipo, fecha)
- **Tipos de Destino**: Beach, Mountain, City, Cultural, Adventure, Relax
- **Validación**: Formularios con validación en tiempo real
- **Responsive Design**: Adaptable a diferentes dispositivos

### **Tecnologías Utilizadas**
- **Angular 21**: Framework principal con componentes standalone
- **TypeScript**: Tipado estático para mayor robustez
- **RxJS**: Programación reactiva para manejo de datos
- **Angular Signals**: Estado reactivo moderno
- **NSwag**: Cliente API generado automáticamente
- **CSS3**: Estilos modernos y responsivos

## Estructura de Carpetas

```
src/app/
├── destinations/           # Gestión de destinos turísticos
│   ├── destinations-page.component.ts    # Lista principal con filtros y CRUD
│   ├── destinations-page.component.html  # Template de la página principal
│   ├── destinations-page.component.css   # Estilos de la página principal
│   ├── destination-detail-page.component.ts    # Vista detallada de destino
│   ├── destination-detail-page.component.html  # Template de detalle
│   └── destination-detail-page.component.css   # Estilos de detalle
├── shared/                 # Componentes y utilidades reutilizables
│   ├── alert/             # Componente de alertas
│   ├── button/            # Componente de botones
│   ├── confirm/           # Componente de confirmación
│   ├── loading/           # Componente de carga
│   └── modal/             # Componente de modales
├── services/              # Servicios de la aplicación
│   └── api-client.ts      # Cliente API generado con NSwag
└── app.*                  # Archivos principales de la app
```

## Arquitectura de Componentes

### **Componentes Standalone**
- **Independientes**: Cada componente es autónomo y reutilizable
- **TypeScript**: Tipado estático para mayor robustez
- **Signals**: Estado reactivo moderno de Angular
- **OnPush**: Estrategia de detección de cambios optimizada

### **Gestión de Estado**
- **Angular Signals**: Estado reactivo para datos de la aplicación
- **RxJS Observables**: Manejo asíncrono de datos de la API
- **Local State**: Estado local en cada componente
- **Event-Driven**: Comunicación entre componentes mediante eventos

### **Servicios y API**
- **ApiClient**: Cliente generado automáticamente con NSwag
- **TypeScript Interfaces**: Tipos seguros para DTOs
- **Error Handling**: Manejo centralizado de errores
- **Loading States**: Estados de carga para mejor UX

### **Componentes Compartidos**
- **AlertComponent**: Notificaciones y mensajes al usuario
- **ButtonComponent**: Botones reutilizables con variantes
- **ModalComponent**: Modales para formularios y confirmaciones
- **LoadingComponent**: Indicadores de carga
- **ConfirmComponent**: Diálogos de confirmación

## Comandos de Desarrollo

### **Instalación**
```bash
npm install
```

### **Desarrollo**
```bash
npm start
# o
ng serve
```

### **Generar Cliente API**
```bash
npm run generate-api
```

### **Build de Producción**
```bash
npm run build
```

### **Tests**
```bash
npm test
# o
ng test
```

## Configuración Técnica

### **NSwag Configuration**
- **Archivo**: `nswag.json`
- **Endpoint**: `http://localhost:5259/swagger/v1/swagger.json`
- **Output**: `src/app/services/api-client.ts`
- **Template**: Angular con HttpClient

### **TypeScript Configuration**
- **Target**: ES2022
- **Strict Mode**: Habilitado
- **Angular**: 21.x
- **Node**: 20.19.x o superior (LTS recomendado)

### **Angular Features**
- **Standalone Components**: Habilitado
- **Signals**: Habilitado
- **Control Flow**: Habilitado (Angular 17+)
- **OnPush Strategy**: Habilitado

## Convenciones de Código

### **Archivos**
- **Componentes**: `component-name.component.ts`
- **Servicios**: `service-name.service.ts`
- **Templates**: `component-name.component.html`
- **Estilos**: `component-name.component.css`

### **Naming**
- **Carpetas**: kebab-case (`destinations-page`)
- **Clases**: PascalCase (`DestinationsPageComponent`)
- **Variables**: camelCase (`selectedDestination`)
- **Constantes**: UPPER_SNAKE_CASE (`API_BASE_URL`)

Esta estructura asegura que el proyecto sea mantenible, escalable y siga las mejores prácticas de Angular moderno.

## Autor

**Lorelay Pricop Florescu**  
Graduada en Tecnologías Interactivas y Project Manager con experiencia en .NET, Python, Angular, Azure DevOps, IA y metodologías ágiles.

🔗 [LinkedIn](https://www.linkedin.com/in/lorelaypricop)  
📧 Contacto: lorelaypricop@gmail.com

## Notas
> Algunas ideas relacionadas con validación, estilo y estructura se revisaron con el apoyo de herramientas de inteligencia artificial (IA), utilizadas para acelerar la documentación y validar casos límite.