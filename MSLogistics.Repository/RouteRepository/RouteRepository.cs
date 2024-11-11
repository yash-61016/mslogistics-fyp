using Microsoft.Extensions.Logging;
using MSLogistics.Application.Repositories.IRouteRepository;
using MSLogistics.Domain;
using MSLogistics.Persistence;
using MSLogistics.Repository.BaseRepository;

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
    }
}
