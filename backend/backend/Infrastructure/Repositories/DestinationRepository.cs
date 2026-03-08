using Microsoft.EntityFrameworkCore;
using backend.Infrastructure.Data;
using backend.Domain.Entities;
using backend.Domain.Interfaces;

namespace backend.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación específica del repositorio de destinos
    /// Proporciona operaciones especializadas para la entidad Destination
    /// </summary>
    public class DestinationRepository : Repository<Destination>, IDestinationRepository
    {
        public DestinationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IPagedResult<Destination>> GetDestinationsWithFiltersAsync(IFilterCriteria filter)
        {
            var query = _dbSet
                .Include(d => d.Type)
                .Include(d => d.Country)
                .AsQueryable();

            // Aplicar filtro de búsqueda por texto (case-insensitive)
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();
                query = query.Where(d => 
                    d.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                    d.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    d.CountryCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Aplicar filtro por código de país específico
            if (!string.IsNullOrWhiteSpace(filter.CountryCode))
            {
                query = query.Where(d => d.CountryCode == filter.CountryCode);
            }

            // Aplicar filtro por tipo de destino (ahora por ID)
            if (filter.DestinationTypeId.HasValue)
            {
                query = query.Where(d => d.DestinationTypeId == filter.DestinationTypeId.Value);
            }

            // Obtener el total de registros antes de aplicar paginación
            var totalCount = await query.CountAsync();

            // Aplicar paginación y ordenamiento
            var destinations = await query
                .OrderByDescending(d => d.LastModif)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<Destination>
            {
                Items = destinations,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<List<string>> GetUniqueCountryCodesAsync()
        {
            return await _dbSet
                .Select(d => d.CountryCode)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<List<Destination>> SearchDestinationsAsync(string searchTerm)
        {
            var trimmedSearchTerm = searchTerm.Trim();
            
            return await _dbSet
                .Where(d => 
                    d.Name.Contains(trimmedSearchTerm, StringComparison.OrdinalIgnoreCase) || 
                    d.Description.Contains(trimmedSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    d.CountryCode.Contains(trimmedSearchTerm, StringComparison.OrdinalIgnoreCase))
                .OrderBy(d => d.Name)
                .ToListAsync();
        }
    }
}
