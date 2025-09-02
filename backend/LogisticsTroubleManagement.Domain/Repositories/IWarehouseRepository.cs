using LogisticsTroubleManagement.Domain.Entities;

namespace LogisticsTroubleManagement.Domain.Repositories
{
    public interface IWarehouseRepository : IRepository<Warehouse>
    {
        Task<IEnumerable<Warehouse>> GetActiveAsync();
        Task<IEnumerable<Warehouse>> GetBySortOrderAsync();
        Task<Warehouse?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}
