using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Infrastructure.Repositories
{
    public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Warehouse>> GetActiveAsync()
        {
            return await _context.Warehouses
                .Where(w => w.IsActive)
                .OrderBy(w => w.SortOrder)
                .ThenBy(w => w.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Warehouse>> GetBySortOrderAsync()
        {
            return await _context.Warehouses
                .OrderBy(w => w.SortOrder)
                .ThenBy(w => w.Name)
                .ToListAsync();
        }

        public async Task<Warehouse?> GetByNameAsync(string name)
        {
            return await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.Warehouses.Where(w => w.Name == name);
            
            if (excludeId.HasValue)
            {
                query = query.Where(w => w.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
