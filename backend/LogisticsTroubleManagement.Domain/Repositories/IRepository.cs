using System.Linq.Expressions;

namespace LogisticsTroubleManagement.Domain.Repositories;

public interface IRepository<T> where T : class
{
    // 基本的なCRUD操作
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task DeleteAsync(int id);
    
    // クエリ操作
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    
    // ページネーション
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null,
        bool ascending = true,
        int page = 1,
        int pageSize = 10);
    
    // 関連データの読み込み
    Task<T?> GetByIdWithIncludeAsync(int id, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes);
}
