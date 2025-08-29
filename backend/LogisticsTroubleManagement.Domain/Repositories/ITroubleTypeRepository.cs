using LogisticsTroubleManagement.Domain.Entities;

namespace LogisticsTroubleManagement.Domain.Repositories
{
    public interface ITroubleTypeRepository : IRepository<TroubleType>
    {
        Task<IEnumerable<TroubleType>> GetActiveAsync();
        Task<IEnumerable<TroubleType>> GetBySortOrderAsync();
        Task<TroubleType?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}
