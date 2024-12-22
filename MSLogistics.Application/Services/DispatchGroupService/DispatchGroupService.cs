using AutoMapper;
using Microsoft.Extensions.Logging;
using MSLogistics.Application.Repositories.IDispatchGroupRepository;
using MSLogistics.Application.ValueObjects.DTOs.DispatchGroups;
using MSLogistics.Application.ValueObjects.Enums;
using MSLogistics.Domain;

namespace MSLogistics.Application.Services.DispatchGroupService
{
	public class DispatchGroupService : IDispatchGroupService
    {
        private readonly IDispatchGroupRepository _dispatchGroupRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DispatchGroupService> _logger;

        public DispatchGroupService(IDispatchGroupRepository dispatchGroupRepository,
            IMapper mapper, ILogger<DispatchGroupService> logger)
        {
            _dispatchGroupRepository = dispatchGroupRepository;
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
                // Map DispatchGroupDto to DispatchGroup
                var dispatchGroupEntities = _mapper.Map<IEnumerable<DispatchGroup>>(dispatchGroupsList);

                // Assign new IDs to each stop entity
                foreach (var dispatchGroup in dispatchGroupEntities)
                {
                    dispatchGroup.Id = Guid.NewGuid();
                }

                // Attempt to add dispatch groups to the repository
                return await _dispatchGroupRepository.AddRangeAsync(dispatchGroupEntities);
            }
            catch (Exception ex)
            {
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception was thrown while inserting a range of records of the {typeof(DispatchGroup)} type.\n" +
                    $"Exception:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

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
            IEnumerable<DispatchGroup> stops = await _dispatchGroupRepository.GetAllWithIncludesAsync(group => group.Routes) ?? new List<DispatchGroup>();

            IEnumerable<DispatchGroupDto> dispatchGroupDtos = _mapper.Map<IEnumerable<DispatchGroupDto>>(stops);

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
                var dispatchGroupToUpdate = new List<DispatchGroup>();

                foreach (var dispatchGroupDto in dispatchGroupsList)
                {
                    // Retrieve the existing stop by ID
                    var existingdispatchGroup = await _dispatchGroupRepository.GetByIdAsync(dispatchGroupDto.Id);

                    if (existingdispatchGroup == null)
                    {
                        _logger.LogError((int)LogEventId.DataAccessError, $"Dispatch group with ID {dispatchGroupDto.Id} not found for updating.");
                        continue;
                    }

                    // Map the new values onto the existing entity
                    _mapper.Map(dispatchGroupDto, existingdispatchGroup);
                    dispatchGroupToUpdate.Add(existingdispatchGroup);
                }

                // If there are any vehicles to update, call UpdateRangeAsync
                if (dispatchGroupToUpdate.Any())
                {
                    return await _dispatchGroupRepository.UpdateRangeAsync(dispatchGroupToUpdate);
                }
                else
                {
                    _logger.LogError((int)LogEventId.DataAccessError, "No valid dispatch group found to update.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Log the exception details with specified format
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception was thrown while updating a range of records of the {typeof(DispatchGroup)} type.\n" +
                    $"Exception:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return false;
            }
        }
    }
}

