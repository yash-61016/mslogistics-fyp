using System.Linq.Expressions;
using MSLogistics.Application.Repositories.IBaseRepository;
using MSLogistics.Domain;

namespace MSLogistics.Application.Repositories.IRouteRepository
{
	public interface IRouteRepository : IBaseRepository<Route>
    {
        Task<Route?> GetRoutesByIdWithIncludesAsync(Guid id, params Expression<Func<Route, object>>[] includeProperties);
    }
}

