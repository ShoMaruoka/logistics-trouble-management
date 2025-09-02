using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Infrastructure.Repositories
{
    public class DamageTypeRepository : Repository<DamageType>, IDamageTypeRepository
    {
        public DamageTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DamageType>> GetActiveAsync()
        {
            return await _context.DamageTypes
                .Where(d => d.IsActive)
                .OrderBy(d => d.SortOrder)
                .ThenBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<DamageType>> GetBySortOrderAsync()
        {
            return await _context.DamageTypes
                .OrderBy(d => d.SortOrder)
                .ThenBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<DamageType>> GetByCategoryAsync(string category)
        {
            return await _context.DamageTypes
                .Where(d => d.Category == category && d.IsActive)
                .OrderBy(d => d.SortOrder)
                .ThenBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<DamageType?> GetByNameAsync(string name)
        {
            return await _context.DamageTypes
                .FirstOrDefaultAsync(d => d.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.DamageTypes.Where(d => d.Name == name);
            
            if (excludeId.HasValue)
            {
                query = query.Where(d => d.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
