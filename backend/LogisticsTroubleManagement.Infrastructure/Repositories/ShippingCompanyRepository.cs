using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Infrastructure.Repositories
{
    public class ShippingCompanyRepository : Repository<ShippingCompany>, IShippingCompanyRepository
    {
        public ShippingCompanyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ShippingCompany>> GetActiveAsync()
        {
            return await _context.ShippingCompanies
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ThenBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<ShippingCompany>> GetBySortOrderAsync()
        {
            return await _context.ShippingCompanies
                .OrderBy(s => s.SortOrder)
                .ThenBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<ShippingCompany>> GetByCompanyTypeAsync(string companyType)
        {
            return await _context.ShippingCompanies
                .Where(s => s.CompanyType == companyType && s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ThenBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<ShippingCompany?> GetByNameAsync(string name)
        {
            return await _context.ShippingCompanies
                .FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.ShippingCompanies.Where(s => s.Name == name);
            
            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
