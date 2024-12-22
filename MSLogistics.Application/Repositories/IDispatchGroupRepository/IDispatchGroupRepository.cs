using System.Linq.Expressions;
using MSLogistics.Application.Repositories.IBaseRepository;
using MSLogistics.Domain;

namespace MSLogistics.Application.Repositories.IDispatchGroupRepository
{
	public interface IDispatchGroupRepository : IBaseRepository<DispatchGroup>
    {
        Task<DispatchGroup?> GetDispatchGroupByIdWithIncludesAsync(Guid id, params Expression<Func<DispatchGroup, object>>[] includeProperties);
    }
}

