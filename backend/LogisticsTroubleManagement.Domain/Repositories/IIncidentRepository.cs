using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Enums;

namespace LogisticsTroubleManagement.Domain.Repositories;

public interface IIncidentRepository : IRepository<Incident>
{
    // インシデント固有のクエリメソッド
    Task<IEnumerable<Incident>> GetByStatusAsync(IncidentStatus status);
    Task<IEnumerable<Incident>> GetByPriorityAsync(Priority priority);
    Task<IEnumerable<Incident>> GetByCategoryAsync(string category);
    Task<IEnumerable<Incident>> GetByReportedByAsync(int userId);
    Task<IEnumerable<Incident>> GetByAssignedToAsync(int userId);
    Task<IEnumerable<Incident>> GetActiveIncidentsAsync();
    Task<IEnumerable<Incident>> GetResolvedIncidentsAsync();
    Task<IEnumerable<Incident>> GetOverdueIncidentsAsync(TimeSpan expectedResolutionTime);
    Task<int> GetCountByStatusAsync(IncidentStatus status);
    Task<int> GetCountByPriorityAsync(Priority priority);
    Task<TimeSpan> GetAverageResolutionTimeAsync();
    Task<decimal> GetPPMAsync(int totalShipments);
}
