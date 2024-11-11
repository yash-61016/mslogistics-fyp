using System.Linq.Expressions;

namespace MSLogistics.Application.Repositories.IBaseRepository
{
	public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>?> GetAllAsync();
        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includeProperties);
        Task<bool> AddAsync(T entity);
        Task<bool> AddRangeAsync(IEnumerable<T> entities);
        Task<bool> UpdateAsync(T entity);
        Task<bool> UpdateRangeAsync(IEnumerable<T> entities);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteRangeAsync(IEnumerable<Guid> ids);
    }
}

