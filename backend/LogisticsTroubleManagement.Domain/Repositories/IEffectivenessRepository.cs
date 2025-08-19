using LogisticsTroubleManagement.Domain.Entities;

namespace LogisticsTroubleManagement.Domain.Repositories;

public interface IEffectivenessRepository : IRepository<Effectiveness>
{
    // 効果測定固有のクエリメソッド
    Task<IEnumerable<Effectiveness>> GetByIncidentIdAsync(int incidentId);
    Task<IEnumerable<Effectiveness>> GetEffectivenessByIncidentAsync(int incidentId);
    Task<IEnumerable<Effectiveness>> GetByTypeAsync(string effectivenessType);
    Task<IEnumerable<Effectiveness>> GetByMeasuredByAsync(int userId);
    Task<IEnumerable<Effectiveness>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<decimal> GetAverageValueByTypeAsync(string effectivenessType);
    Task<decimal> GetTotalValueByTypeAsync(string effectivenessType);
}
