using AutoMapper;
using Microsoft.Extensions.Logging;
using MSLogistics.Application.Repositories.IDispatchGroupRepository;
using MSLogistics.Application.Repositories.IRouteRepository;
using MSLogistics.Application.ValueObjects.DTOs.DispatchGroups;
using MSLogistics.Application.ValueObjects.Enums;
using MSLogistics.Domain;

namespace MSLogistics.Application.Services.DispatchGroupService
{
	public class DispatchGroupService : IDispatchGroupService
    {
        private readonly IDispatchGroupRepository _dispatchGroupRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DispatchGroupService> _logger;

        public DispatchGroupService(IDispatchGroupRepository dispatchGroupRepository,
            IRouteRepository routeRepository,
            IMapper mapper,
            ILogger<DispatchGroupService> logger)
        {
            _dispatchGroupRepository = dispatchGroupRepository;
            _routeRepository = routeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> AddDispatchGroups(IEnumerable<DispatchGroupDto> dispatchGroupsList)
        {
            if (dispatchGroupsList == null || !dispatchGroupsList.Any())
            {
                return false;
            }

            try
            {
                var dispatchGroupEntities = dispatchGroupsList.Select(dto => new DispatchGroup
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    DispatchDate = dto.DispatchDate.ToUniversalTime(),
                    Routes = dto.RoutesIds
                        .Select(routeId => _routeRepository.GetByIdAsync(routeId).Result) // Fetch routes synchronously
                        .Where(route => route != null)
                        .ToList()
                }).ToList();

                return await _dispatchGroupRepository.AddRangeAsync(dispatchGroupEntities);
            }
            catch (Exception ex)
            {
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception while inserting DispatchGroup records: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> DeleteDispatchGroups(List<Guid> Ids)
        {
            if (Ids == null || !Ids.Any())
            {
                return false;
            }

            try
            {
                ICollection<Guid> dispatchGroupsIds = new List<Guid>();

                foreach (Guid Id in Ids)
                {
                    DispatchGroup? dispatchGroup = await _dispatchGroupRepository.GetByIdAsync(Id);

                    if (dispatchGroup != null)
                        dispatchGroupsIds.Add(dispatchGroup.Id);
                    else
                        _logger.LogError((int)LogEventId.DataAccessError, $"Stop with ID {Id} not found in the repository.");
                }

                // Attempt to delete the stops with the specified IDs
                return await _dispatchGroupRepository.DeleteRangeAsync(dispatchGroupsIds);
            }
            catch (Exception ex)
            {
                // Log the exception details with specified format
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception was thrown while deleting a range of records of the {typeof(DispatchGroup)} type.\n" +
                    $"Exception:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return false;
            }
        }

        public async Task<DispatchGroupDto> GetDispatchGroupById(Guid Id)
        {
            DispatchGroup dispatchGroup = await _dispatchGroupRepository.GetDispatchGroupByIdWithIncludesAsync(Id, group => group.Routes) ?? new DispatchGroup();

            DispatchGroupDto dispatchGroupDto = _mapper.Map<DispatchGroupDto>(dispatchGroup);

            return dispatchGroupDto ?? new DispatchGroupDto();
        }

        public async Task<IEnumerable<DispatchGroupDto>> GetDispatchGroups()
        {
            IEnumerable<DispatchGroup> dispatchGroups = await _dispatchGroupRepository.GetAllWithIncludesAsync(group => group.Routes) ?? new List<DispatchGroup>();

            IEnumerable<DispatchGroupDto> dispatchGroupDtos = _mapper.Map<IEnumerable<DispatchGroupDto>>(dispatchGroups);

            return dispatchGroupDtos ?? new List<DispatchGroupDto>();
        }

        public async Task<bool> UpdateDispatchGroups(IEnumerable<DispatchGroupDto> dispatchGroupsList)
        {
            if (dispatchGroupsList == null || !dispatchGroupsList.Any())
            {
                return false;
            }

            try
            {
                var dispatchGroupsToUpdate = new List<DispatchGroup>();

                foreach (var dispatchGroupDto in dispatchGroupsList)
                {
                    var existingDispatchGroup = await _dispatchGroupRepository.GetDispatchGroupByIdWithIncludesAsync(dispatchGroupDto.Id, group => group.Routes);
                    if (existingDispatchGroup == null)
                    {
                        _logger.LogError($"Dispatch group with ID {dispatchGroupDto.Id} not found for updating.");
                        continue;
                    }

                    // Map other properties
                    _mapper.Map(dispatchGroupDto, existingDispatchGroup);

                    // Fetch each route individually and update
                    existingDispatchGroup.Routes.Clear();
                    foreach (var routeId in dispatchGroupDto.RoutesIds)
                    {
                        var route = await _routeRepository.GetByIdAsync(routeId);
                        if (route != null)
                        {
                            existingDispatchGroup.Routes.Add(route);
                        }
                        else
                        {
                            _logger.LogWarning($"Route with ID {routeId} not found. Skipping.");
                        }
                    }

                    dispatchGroupsToUpdate.Add(existingDispatchGroup);
                }

                if (dispatchGroupsToUpdate.Any())
                {
                    return await _dispatchGroupRepository.UpdateRangeAsync(dispatchGroupsToUpdate);
                }

                _logger.LogError("No valid dispatch group found to update.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception while updating DispatchGroups: {ex.Message}");
                return false;
            }
        }

    }
}

