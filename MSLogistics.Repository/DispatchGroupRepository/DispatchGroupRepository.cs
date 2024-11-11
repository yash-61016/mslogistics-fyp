using Microsoft.Extensions.Logging;
using MSLogistics.Application.Repositories.IDispatchGroupRepository;
using MSLogistics.Domain;
using MSLogistics.Persistence;
using MSLogistics.Repository.BaseRepository;

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
    }
}
