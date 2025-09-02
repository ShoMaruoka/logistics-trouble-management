using LogisticsTroubleManagement.Domain.Entities;

namespace LogisticsTroubleManagement.Domain.Repositories
{
    public interface IDamageTypeRepository : IRepository<DamageType>
    {
        Task<IEnumerable<DamageType>> GetActiveAsync();
        Task<IEnumerable<DamageType>> GetBySortOrderAsync();
        Task<IEnumerable<DamageType>> GetByCategoryAsync(string category);
        Task<DamageType?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}
