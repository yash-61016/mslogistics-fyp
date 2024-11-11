using Microsoft.Extensions.Logging;
using MSLogistics.Application.Repositories.IStopRepository;
using MSLogistics.Domain;
using MSLogistics.Persistence;
using MSLogistics.Repository.BaseRepository;

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
    }
}
