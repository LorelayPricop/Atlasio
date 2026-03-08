using MediatR;
using Microsoft.EntityFrameworkCore;
using backend.Infrastructure.Data;
using backend.Application.DTOs;

namespace backend.Application.Queries
{
    /// <summary>
    /// Handler para la query de obtener tipos de destino desde la base de datos
    /// </summary>
    public class GetDestinationTypesQueryHandler : IRequestHandler<GetDestinationTypesQuery, List<DestinationTypeDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetDestinationTypesQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DestinationTypeDto>> Handle(GetDestinationTypesQuery request, CancellationToken cancellationToken)
        {
            var types = await _context.DestinationTypes
                .Where(t => t.IsActive)
                .OrderBy(t => t.DisplayOrder)
                .Select(t => new DestinationTypeDto
                {
                    Id = t.Id,
                    Code = t.Code,
                    Name = t.Name,
                    Icon = t.Icon,
                    ColorBackground = t.ColorBackground,
                    ColorForeground = t.ColorForeground,
                    DisplayOrder = t.DisplayOrder,
                    IsActive = t.IsActive
                })
                .ToListAsync(cancellationToken);

            return types;
        }
    }
}
