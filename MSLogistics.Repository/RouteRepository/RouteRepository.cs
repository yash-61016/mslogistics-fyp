using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MSLogistics.Application.Repositories.IRouteRepository;
using MSLogistics.Domain;
using MSLogistics.Persistence;
using MSLogistics.Repository.BaseRepository;
using MSLogistics.Application.ValueObjects.Enums;

namespace MSLogistics.Repository.RouteRepository
{
	public class RouteRepository : BaseRepository<Route>, IRouteRepository
    {
        private readonly DomainContext _context;
        private readonly ILogger<RouteRepository> _logger;

        public RouteRepository(DomainContext context, ILogger<RouteRepository> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Route?> GetRoutesByIdWithIncludesAsync(Guid id, params Expression<Func<Route, object>>[] includeProperties)
        {
            try
            {
                IQueryable<Route> query = _context.Routes;

                // Dynamically include specified navigation properties
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                // Filter the entity by its ID
                return await query.FirstOrDefaultAsync(entity => entity.Id == id) ?? new Route();
            }
            catch (Exception ex)
            {
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception was thrown while retrieving an entity of the Routes type with all associated includes.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");
                return null;
            }
        }
    }
}
