using MSLogistics.Application.ValueObjects.DTOs.Stop;

namespace MSLogistics.Application.Services.StopService
{
    public interface IStopService
    {
        /// <summary>
        /// Retrieves all stops.
        /// </summary>
        /// <returns>A collection of StopDto representing all stops.</returns>
        Task<IEnumerable<StopDto>> GetStops();

        /// <summary>
        /// Retrieves a specific stop by its unique identifier.
        /// </summary>
        /// <param name="Id">The unique identifier of the stop.</param>
        /// <returns>A StopDto representing the requested stop, or null if not found.</returns>
        Task<StopDto> GetStopById(Guid Id);

        /// <summary>
        /// Adds a collection of new stops.
        /// </summary>
        /// <param name="stopsList">The collection of StopDto representing the stops to add.</param>
        /// <returns>True if the stops were successfully added, false otherwise.</returns>
        Task<bool> AddStops(IEnumerable<StopDto> stopsList);

        /// <summary>
        /// Deletes a collection of stops based on their unique identifiers.
        /// </summary>
        /// <param name="Ids">The list of unique identifiers for the stops to delete.</param>
        /// <returns>True if the stops were successfully deleted, false otherwise.</returns>
        Task<bool> DeleteStops(List<Guid> Ids);

        /// <summary>
        /// Updates a collection of existing stops.
        /// </summary>
        /// <param name="stopsList">The collection of StopDto containing updated information for each stop.</param>
        /// <returns>True if the stops were successfully updated, false otherwise.</returns>
        Task<bool> UpdateStops(IEnumerable<StopDto> stopsList);
    }
}

