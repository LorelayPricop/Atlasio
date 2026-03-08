using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Application.DTOs;
using backend.Infrastructure.Data;
using Asp.Versioning;

namespace backend.Presentation.Controllers
{
    /// <summary>
    /// Controlador para endpoints de catálogos (Countries, DestinationTypes, Cities)
    /// Proporciona datos maestros para formularios y filtros
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CatalogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CatalogController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene el catálogo completo de países
        /// </summary>
        /// <param name="onlyActive">Si es true, retorna solo países activos (default: true)</param>
        /// <returns>Lista de países con código, nombre y región</returns>
        /// <response code="200">Lista de países obtenida exitosamente</response>
        /// <remarks>
        /// Ejemplo de respuesta:
        /// 
        ///     GET /api/v1/catalog/countries
        ///     [
        ///         {
        ///             "code": "ESP",
        ///             "name": "Spain",
        ///             "region": "Europe",
        ///             "isActive": true
        ///         },
        ///         {
        ///             "code": "MEX",
        ///             "name": "Mexico",
        ///             "region": "North America",
        ///             "isActive": true
        ///         }
        ///     ]
        /// </remarks>
        [HttpGet("countries")]
        [ProducesResponseType(typeof(List<CountryDto>), 200)]
        public async Task<ActionResult<List<CountryDto>>> GetCountries([FromQuery] bool onlyActive = true)
        {
            var query = _context.Countries.AsQueryable();

            if (onlyActive)
            {
                query = query.Where(c => c.IsActive);
            }

            var countries = await query
                .OrderBy(c => c.Name)
                .Select(c => new CountryDto
                {
                    Code = c.Code,
                    Name = c.Name,
                    Region = c.Region,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return Ok(countries);
        }

        /// <summary>
        /// Obtiene el catálogo completo de tipos de destino
        /// </summary>
        /// <param name="onlyActive">Si es true, retorna solo tipos activos (default: true)</param>
        /// <returns>Lista de tipos de destino con ID, código, nombre, ícono, colores y orden de visualización</returns>
        /// <response code="200">Lista de tipos de destino obtenida exitosamente</response>
        /// <remarks>
        /// Ejemplo de respuesta:
        /// 
        ///     GET /api/v1/catalog/destination-types
        ///     [
        ///         {
        ///             "id": 1,
        ///             "code": "BEACH",
        ///             "name": "Beach",
        ///             "icon": "beach_access",
        ///             "colorBackground": "#dbeafe",
        ///             "colorForeground": "#0284c7",
        ///             "displayOrder": 1,
        ///             "isActive": true
        ///         },
        ///         {
        ///             "id": 2,
        ///             "code": "MOUNTAIN",
        ///             "name": "Mountain",
        ///             "icon": "terrain",
        ///             "colorBackground": "#d1fae5",
        ///             "colorForeground": "#059669",
        ///             "displayOrder": 2,
        ///             "isActive": true
        ///         }
        ///     ]
        ///     
        /// Los códigos de tipo de destino disponibles son:
        /// - BEACH: Playas y destinos costeros (Azul)
        /// - MOUNTAIN: Montañas y destinos de altura (Verde)
        /// - CITY: Ciudades y metrópolis (Índigo)
        /// - CULTURAL: Sitios de interés cultural e histórico (Púrpura)
        /// - ADVENTURE: Destinos de aventura y deportes extremos (Naranja)
        /// - RELAX: Destinos de relajación y bienestar (Turquesa)
        /// 
        /// Los colores están diseñados para usar directamente en CSS con las propiedades:
        /// - colorBackground: Para el fondo del badge/chip
        /// - colorForeground: Para el texto y el icono
        /// </remarks>
        [HttpGet("destination-types")]
        [ProducesResponseType(typeof(List<DestinationTypeDto>), 200)]
        public async Task<ActionResult<List<DestinationTypeDto>>> GetDestinationTypes([FromQuery] bool onlyActive = true)
        {
            var query = _context.DestinationTypes.AsQueryable();

            if (onlyActive)
            {
                query = query.Where(dt => dt.IsActive);
            }

            var types = await query
                .OrderBy(dt => dt.DisplayOrder)
                .Select(dt => new DestinationTypeDto
                {
                    Id = dt.Id,
                    Code = dt.Code,
                    Name = dt.Name,
                    Icon = dt.Icon,
                    ColorBackground = dt.ColorBackground,
                    ColorForeground = dt.ColorForeground,
                    DisplayOrder = dt.DisplayOrder,
                    IsActive = dt.IsActive
                })
                .ToListAsync();

            return Ok(types);
        }

        /// <summary>
        /// Obtiene el catálogo de ciudades, opcionalmente filtrado por país
        /// </summary>
        /// <param name="countryCode">Código de país ISO 3166-1 alpha-3 (opcional)</param>
        /// <param name="onlyActive">Si es true, retorna solo ciudades activas (default: true)</param>
        /// <returns>Lista de ciudades con ID, nombre y código de país</returns>
        /// <response code="200">Lista de ciudades obtenida exitosamente</response>
        /// <remarks>
        /// Ejemplos de uso:
        /// 
        ///     GET /api/v1/catalog/cities
        ///     GET /api/v1/catalog/cities?countryCode=ESP
        ///     GET /api/v1/catalog/cities?countryCode=MEX&amp;onlyActive=false
        ///     
        /// Ejemplo de respuesta:
        /// 
        ///     [
        ///         {
        ///             "id": 1,
        ///             "countryCode": "ESP",
        ///             "name": "Barcelona",
        ///             "isActive": true
        ///         },
        ///         {
        ///             "id": 2,
        ///             "countryCode": "ESP",
        ///             "name": "Madrid",
        ///             "isActive": true
        ///         }
        ///     ]
        /// </remarks>
        [HttpGet("cities")]
        [ProducesResponseType(typeof(List<CityDto>), 200)]
        public async Task<ActionResult<List<CityDto>>> GetCities(
            [FromQuery] string? countryCode = null,
            [FromQuery] bool onlyActive = true)
        {
            var query = _context.Cities.AsQueryable();

            if (!string.IsNullOrWhiteSpace(countryCode))
            {
                query = query.Where(c => c.CountryCode == countryCode.ToUpper());
            }

            if (onlyActive)
            {
                query = query.Where(c => c.IsActive);
            }

            var cities = await query
                .OrderBy(c => c.Name)
                .Select(c => new CityDto
                {
                    Id = c.Id,
                    CountryCode = c.CountryCode,
                    Name = c.Name,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return Ok(cities);
        }

        /// <summary>
        /// Obtiene un país específico por su código
        /// </summary>
        /// <param name="code">Código de país ISO 3166-1 alpha-3 (ej: ESP, USA, MEX)</param>
        /// <returns>Información del país</returns>
        /// <response code="200">País encontrado</response>
        /// <response code="404">País no encontrado</response>
        [HttpGet("countries/{code}")]
        [ProducesResponseType(typeof(CountryDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CountryDto>> GetCountry(string code)
        {
            var country = await _context.Countries
                .Where(c => c.Code == code.ToUpper())
                .Select(c => new CountryDto
                {
                    Code = c.Code,
                    Name = c.Name,
                    Region = c.Region,
                    IsActive = c.IsActive
                })
                .FirstOrDefaultAsync();

            if (country == null)
            {
                return NotFound(new { message = $"País con código '{code}' no encontrado" });
            }

            return Ok(country);
        }

        /// <summary>
        /// Obtiene un tipo de destino específico por su ID
        /// </summary>
        /// <param name="id">ID del tipo de destino</param>
        /// <returns>Información del tipo de destino</returns>
        /// <response code="200">Tipo de destino encontrado</response>
        /// <response code="404">Tipo de destino no encontrado</response>
        [HttpGet("destination-types/{id}")]
        [ProducesResponseType(typeof(DestinationTypeDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DestinationTypeDto>> GetDestinationType(int id)
        {
            var type = await _context.DestinationTypes
                .Where(dt => dt.Id == id)
                .Select(dt => new DestinationTypeDto
                {
                    Id = dt.Id,
                    Code = dt.Code,
                    Name = dt.Name,
                    Icon = dt.Icon,
                    ColorBackground = dt.ColorBackground,
                    ColorForeground = dt.ColorForeground,
                    DisplayOrder = dt.DisplayOrder,
                    IsActive = dt.IsActive
                })
                .FirstOrDefaultAsync();

            if (type == null)
            {
                return NotFound(new { message = $"Tipo de destino con ID {id} no encontrado" });
            }

            return Ok(type);
        }
    }
}
