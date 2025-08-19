using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Infrastructure.Repositories;

public class AttachmentRepository : Repository<Attachment>, IAttachmentRepository
{
    public AttachmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Attachment>> GetByIncidentIdAsync(int incidentId)
    {
        return await _dbSet.Where(a => a.IncidentId == incidentId).ToListAsync();
    }

    public async Task<IEnumerable<Attachment>> GetAttachmentsByIncidentAsync(int incidentId)
    {
        return await _dbSet.Where(a => a.IncidentId == incidentId).ToListAsync();
    }

    public async Task<IEnumerable<Attachment>> GetByUploadedByAsync(int userId)
    {
        return await _dbSet.Where(a => a.UploadedById == userId).ToListAsync();
    }

    public async Task<IEnumerable<Attachment>> GetByContentTypeAsync(string contentType)
    {
        return await _dbSet.Where(a => a.ContentType == contentType).ToListAsync();
    }

    public async Task<long> GetTotalFileSizeAsync()
    {
        return await _dbSet.SumAsync(a => a.FileSize);
    }

    public async Task<long> GetTotalFileSizeByIncidentAsync(int incidentId)
    {
        return await _dbSet.Where(a => a.IncidentId == incidentId).SumAsync(a => a.FileSize);
    }

    public async Task<IEnumerable<Attachment>> GetLargeFilesAsync(long minSizeInBytes)
    {
        return await _dbSet.Where(a => a.FileSize >= minSizeInBytes).ToListAsync();
    }
}
