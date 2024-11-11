
using Microsoft.Extensions.Logging;
using MSLogistics.Application.Repositories.IVehicleRepository;
using MSLogistics.Domain;
using MSLogistics.Persistence;
using MSLogistics.Repository.BaseRepository;

namespace MSLogistics.Repository.VehicleRepository
{
	public class VehicleRepository : BaseRepository<Vehicle> , IVehicleRepository
	{
        private readonly DomainContext _context;
        private readonly ILogger<VehicleRepository> _logger;

        public VehicleRepository(DomainContext context, ILogger<VehicleRepository> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
    }
}

