using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Infrastructure.Repositories
{
    public class TroubleTypeRepository : Repository<TroubleType>, ITroubleTypeRepository
    {
        public TroubleTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TroubleType>> GetActiveAsync()
        {
            return await _context.TroubleTypes
                .Where(t => t.IsActive)
                .OrderBy(t => t.SortOrder)
                .ThenBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<TroubleType>> GetBySortOrderAsync()
        {
            return await _context.TroubleTypes
                .OrderBy(t => t.SortOrder)
                .ThenBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<TroubleType?> GetByNameAsync(string name)
        {
            return await _context.TroubleTypes
                .FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.TroubleTypes.Where(t => t.Name == name);
            
            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
