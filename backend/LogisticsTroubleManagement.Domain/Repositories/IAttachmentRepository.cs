using LogisticsTroubleManagement.Domain.Entities;

namespace LogisticsTroubleManagement.Domain.Repositories;

public interface IAttachmentRepository : IRepository<Attachment>
{
    // 添付ファイル固有のクエリメソッド
    Task<IEnumerable<Attachment>> GetByIncidentIdAsync(int incidentId);
    Task<IEnumerable<Attachment>> GetByUploadedByAsync(int userId);
    Task<IEnumerable<Attachment>> GetByContentTypeAsync(string contentType);
    Task<long> GetTotalFileSizeAsync();
    Task<long> GetTotalFileSizeByIncidentAsync(int incidentId);
    Task<IEnumerable<Attachment>> GetLargeFilesAsync(long minSizeInBytes);
}
