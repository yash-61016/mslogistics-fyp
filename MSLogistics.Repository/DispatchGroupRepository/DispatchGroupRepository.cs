using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MSLogistics.Application.Repositories.IDispatchGroupRepository;
using MSLogistics.Domain;
using MSLogistics.Persistence;
using MSLogistics.Repository.BaseRepository;
using MSLogistics.Application.ValueObjects.Enums;

namespace MSLogistics.Repository.DispatchGroupRepository
{
	public class DispatchGroupRepository : BaseRepository<DispatchGroup>, IDispatchGroupRepository
    {
        private readonly DomainContext _context;
        private readonly ILogger<DispatchGroupRepository> _logger;

        public DispatchGroupRepository(DomainContext context, ILogger<DispatchGroupRepository> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DispatchGroup?> GetDispatchGroupByIdWithIncludesAsync(Guid id, params Expression<Func<DispatchGroup, object>>[] includeProperties)
        {
            try
            {
                IQueryable<DispatchGroup> query = _context.DispatchGroups;

                // Dynamically include specified navigation properties
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                // Filter the entity by its ID
                return await query.FirstOrDefaultAsync(entity => entity.Id == id) ?? new DispatchGroup();
            }
            catch (Exception ex)
            {
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception was thrown while retrieving an entity of the DispatchGroup type with all associated includes.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");
                return null;
            }
        }
    }
}
