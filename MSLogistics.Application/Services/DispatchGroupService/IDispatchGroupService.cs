using System;
using MSLogistics.Application.ValueObjects.DTOs.DispatchGroups;

namespace MSLogistics.Application.Services.DispatchGroupService
{
	public interface IDispatchGroupService
	{
        /// <summary>
        /// Retrieves all dispatch groups.
        /// </summary>
        /// <returns>A collection of DispatchGroupDto representing all dispatch group.</returns>
        Task<IEnumerable<DispatchGroupDto>> GetDispatchGroups();

        /// <summary>
        /// Retrieves a specific dispatch group by its unique identifier.
        /// </summary>
        /// <param name="Id">The unique identifier of the stop.</param>
        /// <returns>A DispatchGroupDto representing the requested dispatch group, or null if not found.</returns>
        Task<DispatchGroupDto> GetDispatchGroupById(Guid Id);

        /// <summary>
        /// Adds a collection of new dispatch groups.
        /// </summary>
        /// <param name="dispatchGroupsList">The collection of DispatchGroupDto representing the dispatch group to add.</param>
        /// <returns>True if the dispatch group were successfully added, false otherwise.</returns>
        Task<bool> AddDispatchGroups(IEnumerable<DispatchGroupDto> dispatchGroupsList);

        /// <summary>
        /// Deletes a collection of dispatch group based on their unique identifiers.
        /// </summary>
        /// <param name="Ids">The list of unique identifiers for the dispatch group to delete.</param>
        /// <returns>True if the dispatch group were successfully deleted, false otherwise.</returns>
        Task<bool> DeleteDispatchGroups(List<Guid> Ids);

        /// <summary>
        /// Updates a collection of existing dispatch groups.
        /// </summary>
        /// <param name="dispatchGroupsList">The collection of DispatchGroupDto containing updated information for each dispatch group.</param>
        /// <returns>True if the dispatch group were successfully updated, false otherwise.</returns>
        Task<bool> UpdateDispatchGroups(IEnumerable<DispatchGroupDto> dispatchGroupsList);
    }
}

