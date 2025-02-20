using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MSLogistics.Application.Repositories.IStopRepository;
using MSLogistics.Domain;
using MSLogistics.Persistence;
using MSLogistics.Repository.BaseRepository;
using MSLogistics.Repository.ValueObjects.Enums;

namespace MSLogistics.Repository.StopRepository
{
	public class StopRepository : BaseRepository<Stop>, IStopRepository
    {
        private readonly DomainContext _context;
        private readonly ILogger<StopRepository> _logger;

        public StopRepository(DomainContext context, ILogger<StopRepository> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Stop?> GetStopByIdWithIncludesAsync(Guid id, params Expression<Func<Stop, object>>[] includeProperties)
        {
            try
            {
                IQueryable<Stop> query = _context.Stops;

                // Dynamically include specified navigation properties
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                // Filter the entity by its ID
                return await query.FirstOrDefaultAsync(entity => entity.Id == id) ?? new Stop();
            }
            catch (Exception ex)
            {
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception was thrown while retrieving an entity of the stops with id {id} with all associated includes.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");
                return null;
            }
        }
    }
}
