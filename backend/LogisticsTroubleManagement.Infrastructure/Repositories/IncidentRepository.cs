using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Enums;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Infrastructure.Repositories;

public class IncidentRepository : Repository<Incident>, IIncidentRepository
{
    public IncidentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Incident>> GetByStatusAsync(IncidentStatus status)
    {
        return await _dbSet.Where(i => i.Status == status).ToListAsync();
    }

    public async Task<IEnumerable<Incident>> GetByPriorityAsync(Priority priority)
    {
        return await _dbSet.Where(i => i.Priority == priority).ToListAsync();
    }

    public async Task<IEnumerable<Incident>> GetByCategoryAsync(string category)
    {
        return await _dbSet.Where(i => i.Category == category).ToListAsync();
    }

    public async Task<IEnumerable<Incident>> GetByReportedByAsync(int userId)
    {
        return await _dbSet.Where(i => i.ReportedById == userId).ToListAsync();
    }

    public async Task<IEnumerable<Incident>> GetByAssignedToAsync(int userId)
    {
        return await _dbSet.Where(i => i.AssignedToId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Incident>> GetActiveIncidentsAsync()
    {
        return await _dbSet.Where(i => i.Status == IncidentStatus.Open || i.Status == IncidentStatus.InProgress).ToListAsync();
    }

    public async Task<IEnumerable<Incident>> GetResolvedIncidentsAsync()
    {
        return await _dbSet.Where(i => i.Status == IncidentStatus.Resolved || i.Status == IncidentStatus.Closed).ToListAsync();
    }

    public async Task<IEnumerable<Incident>> GetOverdueIncidentsAsync(TimeSpan expectedResolutionTime)
    {
        var cutoffDate = DateTime.UtcNow.Subtract(expectedResolutionTime);
        return await _dbSet
            .Where(i => i.Status != IncidentStatus.Resolved && 
                       i.Status != IncidentStatus.Closed && 
                       i.ReportedDate < cutoffDate)
            .ToListAsync();
    }

    public async Task<int> GetCountByStatusAsync(IncidentStatus status)
    {
        return await _dbSet.CountAsync(i => i.Status == status);
    }

    public async Task<int> GetCountByPriorityAsync(Priority priority)
    {
        return await _dbSet.CountAsync(i => i.Priority == priority);
    }

    public async Task<TimeSpan> GetAverageResolutionTimeAsync()
    {
        var resolvedIncidents = await _dbSet
            .Where(i => i.ResolvedDate.HasValue)
            .ToListAsync();

        if (!resolvedIncidents.Any())
            return TimeSpan.Zero;

        var totalTicks = resolvedIncidents.Sum(i => (i.ResolvedDate!.Value - i.ReportedDate).Ticks);
        var averageTicks = totalTicks / resolvedIncidents.Count;

        return TimeSpan.FromTicks(averageTicks);
    }

    public async Task<decimal> GetPPMAsync(int totalShipments)
    {
        if (totalShipments <= 0)
            return 0;

        var resolvedIncidents = await _dbSet
            .Where(i => i.Status == IncidentStatus.Resolved || i.Status == IncidentStatus.Closed)
            .CountAsync();

        return (decimal)resolvedIncidents / totalShipments * 1000000;
    }
}
