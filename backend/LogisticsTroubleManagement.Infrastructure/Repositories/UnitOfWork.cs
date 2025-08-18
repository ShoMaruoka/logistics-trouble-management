using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace LogisticsTroubleManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed = false;

    // リポジトリのインスタンス
    private IUserRepository? _userRepository;
    private IIncidentRepository? _incidentRepository;
    private IAttachmentRepository? _attachmentRepository;
    private IAuditLogRepository? _auditLogRepository;
    private IEffectivenessRepository? _effectivenessRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // リポジトリのプロパティ
    public IUserRepository Users => _userRepository ??= new UserRepository(_context);
    public IIncidentRepository Incidents => _incidentRepository ??= new IncidentRepository(_context);
    public IAttachmentRepository Attachments => _attachmentRepository ??= new AttachmentRepository(_context);
    public IAuditLogRepository AuditLogs => _auditLogRepository ??= new AuditLogRepository(_context);
    public IEffectivenessRepository Effectiveness => _effectivenessRepository ??= new EffectivenessRepository(_context);

    // トランザクション管理
    public async Task BeginTransactionAsync()
    {
        if (_transaction == null)
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        catch
        {
            // ロールバック中のエラーは無視
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    // IDisposable実装
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
        _disposed = true;
    }
}
