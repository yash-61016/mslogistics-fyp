using System;
using System.Linq.Expressions;
using MSLogistics.Application.Repositories.IBaseRepository;
using MSLogistics.Domain;

namespace MSLogistics.Application.Repositories.IStopRepository
{
	public interface IStopRepository : IBaseRepository<Stop>
    {
        Task<Stop?> GetStopByIdWithIncludesAsync(Guid id, params Expression<Func<Stop, object>>[] includeProperties);
    }
}

