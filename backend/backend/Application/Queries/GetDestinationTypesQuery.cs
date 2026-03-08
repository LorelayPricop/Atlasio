using MediatR;
using backend.Application.DTOs;

namespace backend.Application.Queries
{
    /// <summary>
    /// Query para obtener la lista de tipos de destino disponibles desde la base de datos
    /// </summary>
    public class GetDestinationTypesQuery : IRequest<List<DestinationTypeDto>>
    {
    }
}
