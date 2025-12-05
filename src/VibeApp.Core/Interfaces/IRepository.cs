using System.Linq.Expressions;
using VibeApp.Core.Entities;

namespace VibeApp.Core.Interfaces;

/// <summary>
/// Generic repository interface for data access
/// </summary>
public interface IRepository<T> where T : class, IEntity
{
    Task<T?> GetByIdAsync(int id);
    IQueryable<T> GetQueryable();
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

