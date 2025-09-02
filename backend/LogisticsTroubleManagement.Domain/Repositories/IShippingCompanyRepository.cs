using LogisticsTroubleManagement.Domain.Entities;

namespace LogisticsTroubleManagement.Domain.Repositories
{
    public interface IShippingCompanyRepository : IRepository<ShippingCompany>
    {
        Task<IEnumerable<ShippingCompany>> GetActiveAsync();
        Task<IEnumerable<ShippingCompany>> GetBySortOrderAsync();
        Task<IEnumerable<ShippingCompany>> GetByCompanyTypeAsync(string companyType);
        Task<ShippingCompany?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}
