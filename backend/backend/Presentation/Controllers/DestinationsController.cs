using Microsoft.AspNetCore.Mvc;
using backend.Application.DTOs;
using backend.Application.Commands;
using backend.Application.Queries;
using MediatR;
using Serilog;
using Asp.Versioning;

namespace backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class DestinationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DestinationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtiene todos los destinos con filtros y paginación
        /// </summary>
        /// <param name="filter">Filtros de búsqueda y paginación</param>
        /// <returns>Lista paginada de destinos</returns>
        /// <response code="200">Lista de destinos obtenida exitosamente</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<DestinationDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PagedResultDto<DestinationDto>>> GetDestinations(
            [FromQuery] DestinationFilterDto filter)
        {
            var query = new GetDestinationsQuery { Filter = filter };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un destino por su ID
        /// </summary>
        /// <param name="id">ID del destino</param>
        /// <returns>Información del destino</returns>
        /// <response code="200">Destino encontrado exitosamente</response>
        /// <response code="404">Destino no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DestinationDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DestinationDto>> GetDestination(int id)
        {
            var query = new GetDestinationByIdQuery { Id = id };
            var destination = await _mediator.Send(query);
            
            if (destination is null)
            {
                Log.Warning("Destino no encontrado con ID: {DestinationId}", id);
                return NotFound(new { message = "Destino no encontrado" });
            }
            
            return Ok(destination);
        }

        /// <summary>
        /// Crea un nuevo destino
        /// </summary>
        /// <param name="createDto">Datos del nuevo destino</param>
        /// <returns>Destino creado</returns>
        /// <response code="201">Destino creado exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(DestinationDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DestinationDto>> CreateDestination(CreateDestinationDto createDto)
        {
            if (!ModelState.IsValid)
            {
                Log.Warning("Datos de entrada inválidos para crear destino: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            var command = new CreateDestinationCommand { CreateDestinationDto = createDto };
            var destination = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetDestination), new { id = destination.ID }, destination);
        }

        /// <summary>
        /// Actualiza un destino existente
        /// </summary>
        /// <param name="id">ID del destino a actualizar</param>
        /// <param name="updateDto">Datos actualizados del destino</param>
        /// <returns>Destino actualizado</returns>
        /// <response code="200">Destino actualizado exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="404">Destino no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DestinationDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DestinationDto>> UpdateDestination(int id, UpdateDestinationDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                Log.Warning("Datos de entrada inválidos para actualizar destino ID {DestinationId}: {@ModelState}", id, ModelState);
                return BadRequest(ModelState);
            }

            var command = new UpdateDestinationCommand { Id = id, UpdateDestinationDto = updateDto };
            var destination = await _mediator.Send(command);
            
            if (destination is null)
            {
                Log.Warning("Destino no encontrado para actualizar con ID: {DestinationId}", id);
                return NotFound(new { message = "Destino no encontrado" });
            }
            
            return Ok(destination);
        }

        /// <summary>
        /// Elimina un destino
        /// </summary>
        /// <param name="id">ID del destino a eliminar</param>
        /// <returns>Sin contenido</returns>
        /// <response code="204">Destino eliminado exitosamente</response>
        /// <response code="404">Destino no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteDestination(int id)
        {
            var command = new DeleteDestinationCommand { Id = id };
            var result = await _mediator.Send(command);
            
            if (!result)
            {
                Log.Warning("Destino no encontrado para eliminar con ID: {DestinationId}", id);
                return NotFound(new { message = "Destino no encontrado" });
            }

            return NoContent();
        }

        /// <summary>
        /// Obtiene la lista de códigos de países disponibles
        /// </summary>
        /// <returns>Lista de códigos de países</returns>
        /// <response code="200">Lista de países obtenida exitosamente</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("countries")]
        [ProducesResponseType(typeof(List<string>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<string>>> GetCountries()
        {
            var query = new GetCountriesQuery();
            var countries = await _mediator.Send(query);
            return Ok(countries);
        }

        /// <summary>
        /// Obtiene la lista de tipos de destino disponibles
        /// </summary>
        /// <returns>Lista de tipos de destino</returns>
        /// <response code="200">Lista de tipos obtenida exitosamente</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("types")]
        [ProducesResponseType(typeof(List<string>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<string>>> GetDestinationTypes()
        {
            var query = new GetDestinationTypesQuery();
            var types = await _mediator.Send(query);
            return Ok(types);
        }
    }
}
