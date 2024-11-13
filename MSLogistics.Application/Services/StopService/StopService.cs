using AutoMapper;
using Microsoft.Extensions.Logging;
using MSLogistics.Application.Repositories.IStopRepository;
using MSLogistics.Application.ValueObjects.DTOs.Stop;
using MSLogistics.Application.ValueObjects.Enums;
using MSLogistics.Domain;

namespace MSLogistics.Application.Services.StopService
{
	public class StopService : IStopService
	{
        private readonly IStopRepository _stopRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<StopService> _logger;

        public StopService(IStopRepository stopRepository,
            IMapper mapper, ILogger<StopService> logger)
        {
            _stopRepository = stopRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> AddStops(IEnumerable<StopDto> stopsList)
        {
            if (stopsList == null || !stopsList.Any())
            {
                return false;
            }

            try
            {
                // Map StopDto to Stop
                var stopEntities = _mapper.Map<IEnumerable<Stop>>(stopsList);

                // Assign new IDs to each stop entity
                foreach (var stop in stopEntities)
                {
                    stop.Id = Guid.NewGuid();
                }

                // Attempt to add stops to the repository
                return await _stopRepository.AddRangeAsync(stopEntities);
            }
            catch (Exception ex)
            {
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception was thrown while inserting a range of records of the {typeof(Stop)} type.\n" +
                    $"Exception:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return false;
            }
        }

        public async Task<bool> DeleteStops(List<Guid> Ids)
        {
            if (Ids == null || !Ids.Any())
            {
                return false;
            }

            try
            {
                ICollection<Guid> stopsIds = new List<Guid>();

                foreach (Guid Id in Ids)
                {
                    Stop? stop = await _stopRepository.GetByIdAsync(Id);

                    if (stop != null)
                        stopsIds.Add(stop.Id);
                    else
                        _logger.LogError((int)LogEventId.DataAccessError, $"Stop with ID {Id} not found in the repository.");
                }

                // Attempt to delete the stops with the specified IDs
                return await _stopRepository.DeleteRangeAsync(stopsIds);
            }
            catch (Exception ex)
            {
                // Log the exception details with specified format
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception was thrown while deleting a range of records of the {typeof(Stop)} type.\n" +
                    $"Exception:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return false;
            }
        }

        public async Task<StopDto> GetStopById(Guid Id)
        {
            Stop stop = await _stopRepository.GetByIdAsync(Id) ?? new Stop();

            StopDto stopDto = _mapper.Map<StopDto>(stop);

            return stopDto ?? new StopDto();
        }

        public async Task<IEnumerable<StopDto>> GetStops()
        {
            IEnumerable<Stop> stops = await _stopRepository.GetAllAsync() ?? new List<Stop>();

            IEnumerable<StopDto> stopsDtos = _mapper.Map<IEnumerable<StopDto>>(stops);

            return stopsDtos ?? new List<StopDto>();
        }

        public async Task<bool> UpdateStops(IEnumerable<StopDto> stopsList)
        {
            if (stopsList == null || !stopsList.Any())
            {
                return false;
            }

            try
            {
                var stopsToUpdate = new List<Stop>();

                foreach (var stopDto in stopsList)
                {
                    // Retrieve the existing stop by ID
                    var existingStop = await _stopRepository.GetByIdAsync(stopDto.Id);

                    if (existingStop == null)
                    {
                        _logger.LogError((int)LogEventId.DataAccessError, $"Stop with ID {stopDto.Id} not found for updating.");
                        continue;
                    }

                    // Map the new values onto the existing entity
                    _mapper.Map(stopDto, existingStop);
                    stopsToUpdate.Add(existingStop);
                }

                // If there are any vehicles to update, call UpdateRangeAsync
                if (stopsToUpdate.Any())
                {
                    return await _stopRepository.UpdateRangeAsync(stopsToUpdate);
                }
                else
                {
                    _logger.LogError((int)LogEventId.DataAccessError, "No valid stops found to update.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Log the exception details with specified format
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception was thrown while updating a range of records of the {typeof(Stop)} type.\n" +
                    $"Exception:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return false;
            }
        }
    }
}

