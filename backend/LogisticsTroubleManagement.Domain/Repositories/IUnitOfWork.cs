namespace LogisticsTroubleManagement.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    // リポジトリの取得
    IUserRepository Users { get; }
    IIncidentRepository Incidents { get; }
    IAttachmentRepository Attachments { get; }
    IAuditLogRepository AuditLogs { get; }
    IEffectivenessRepository Effectiveness { get; }
    
    // トランザクション管理
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
