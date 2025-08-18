using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Infrastructure.Repositories;

public class EffectivenessRepository : Repository<Effectiveness>, IEffectivenessRepository
{
    public EffectivenessRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Effectiveness>> GetByIncidentIdAsync(int incidentId)
    {
        return await _dbSet.Where(e => e.IncidentId == incidentId).ToListAsync();
    }

    public async Task<IEnumerable<Effectiveness>> GetByTypeAsync(string effectivenessType)
    {
        return await _dbSet.Where(e => e.EffectivenessType == effectivenessType).ToListAsync();
    }

    public async Task<IEnumerable<Effectiveness>> GetByMeasuredByAsync(int userId)
    {
        return await _dbSet.Where(e => e.MeasuredById == userId).ToListAsync();
    }

    public async Task<IEnumerable<Effectiveness>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet.Where(e => e.MeasuredAt >= startDate && e.MeasuredAt <= endDate).ToListAsync();
    }

    public async Task<decimal> GetAverageValueByTypeAsync(string effectivenessType)
    {
        return await _dbSet
            .Where(e => e.EffectivenessType == effectivenessType)
            .AverageAsync(e => e.Value);
    }

    public async Task<decimal> GetTotalValueByTypeAsync(string effectivenessType)
    {
        return await _dbSet
            .Where(e => e.EffectivenessType == effectivenessType)
            .SumAsync(e => e.Value);
    }
}
