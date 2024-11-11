using AutoMapper;
using Microsoft.Extensions.Logging;
using MSLogistics.Application.Repositories.IVehicleRepository;
using MSLogistics.Application.ValueObjects.DTOs.Vehicle;
using MSLogistics.Application.ValueObjects.Enums;
using MSLogistics.Domain;

namespace MSLogistics.Application.Services.VehicleService
{
	public class VehicleService : IVehicleService
	{
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<VehicleService> _logger;

		public VehicleService(IVehicleRepository vehicleRepository,
            IMapper mapper, ILogger<VehicleService> logger)
		{
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
            _logger = logger;
		}

        public async Task<bool> AddVehicles(IEnumerable<VehicleDto> vehicleList)
        {
            if (vehicleList == null || !vehicleList.Any())
            {
                return false;
            }

            try
            {
                // Map VehicleDto to Vehicle
                var vehicleEntities = _mapper.Map<IEnumerable<Vehicle>>(vehicleList);

                // Assign new IDs to each vehicle entity
                foreach (var vehicle in vehicleEntities)
                {
                    vehicle.Id = Guid.NewGuid();
                }

                // Attempt to add vehicles to the repository
                return await _vehicleRepository.AddRangeAsync(vehicleEntities);
            }
            catch (Exception ex)
            {
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception was thrown while inserting a range of records of the {typeof(Vehicle)} type.\n" +
                    $"Exception:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return false;
            }
        }

        public async Task<bool> DeleteVehicles(List<Guid> Ids)
        {
            if (Ids == null || !Ids.Any())
            {
                return false;
            }

            try
            {
                ICollection<Guid> vehiclesId = new List<Guid>();

                foreach (Guid Id in Ids)
                {
                    Vehicle? vehicle = await _vehicleRepository.GetByIdAsync(Id);

                    if (vehicle != null)
                        vehiclesId.Add(vehicle.Id);
                    else
                        _logger.LogError((int)LogEventId.DataAccessError, $"Vehicle with ID {Id} not found in the repository.");
                }

                // Attempt to delete the vehicles with the specified IDs
                return await _vehicleRepository.DeleteRangeAsync(vehiclesId);
            }
            catch (Exception ex)
            {
                // Log the exception details with specified format
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception was thrown while deleting a range of records of the {typeof(Vehicle)} type.\n" +
                    $"Exception:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return false;
            }
        }

        public async Task<VehicleDto> GetVehicleById(Guid Id)
        {
            Vehicle vehicle = await _vehicleRepository.GetByIdAsync(Id) ?? new Vehicle();

            VehicleDto vehicleDto = _mapper.Map<VehicleDto>(vehicle);

            return vehicleDto ?? new VehicleDto();
        }

        public async Task<IEnumerable<VehicleDto>> GetVehicles()
        {
            IEnumerable<Vehicle> vehicles = await _vehicleRepository.GetAllAsync() ?? new List<Vehicle>();

            IEnumerable<VehicleDto> vehicleDtos = _mapper.Map<IEnumerable<VehicleDto>>(vehicles);

            return vehicleDtos ?? new List<VehicleDto>();
        }

        public async Task<bool> UpdateVehicles(IEnumerable<VehicleDto> vehicleList)
        {
            if (vehicleList == null || !vehicleList.Any())
            {
                return false;
            }

            try
            {
                var vehiclesToUpdate = new List<Vehicle>();

                foreach (var vehicleDto in vehicleList)
                {
                    // Retrieve the existing vehicle by ID
                    var existingVehicle = await _vehicleRepository.GetByIdAsync(vehicleDto.Id);

                    if (existingVehicle == null)
                    {
                        _logger.LogError((int)LogEventId.DataAccessError, $"Vehicle with ID {vehicleDto.Id} not found for updating.");
                        continue;
                    }

                    // Map the new values onto the existing entity
                    _mapper.Map(vehicleDto, existingVehicle);
                    vehiclesToUpdate.Add(existingVehicle);
                }

                // If there are any vehicles to update, call UpdateRangeAsync
                if (vehiclesToUpdate.Any())
                {
                    return await _vehicleRepository.UpdateRangeAsync(vehiclesToUpdate);
                }
                else
                {
                    _logger.LogError((int)LogEventId.DataAccessError, "No valid vehicles found to update.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Log the exception details with specified format
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception was thrown while updating a range of records of the {typeof(Vehicle)} type.\n" +
                    $"Exception:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return false;
            }
        }
    }
}