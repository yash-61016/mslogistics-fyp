using MSLogistics.Application.ValueObjects.DTOs.Route;

namespace MSLogistics.Application.Services.RouteService
{
    public interface IRouteService
    {
        /// <summary>
        /// Retrieves a list of all routes.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a collection of <see cref="RouteDto"/>.</returns>
        Task<IEnumerable<RouteDto>> GetRoutes();

        /// <summary>
        /// Retrieves a specific route by its unique identifier.
        /// </summary>
        /// <param name="Id">The unique identifier of the route.</param>
        /// <returns>A task representing the asynchronous operation, containing the corresponding <see cref="RouteDto"/>.</returns>
        Task<RouteDto> GetRouteById(Guid Id);

        /// <summary>
        /// Adds a collection of routes to the system.
        /// </summary>
        /// <param name="routesList">The collection of routes to be added.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean value indicating success or failure.</returns>
        Task<bool> AddRoutes(IEnumerable<RouteDto> routesList);

        /// <summary>
        /// Deletes a collection of routes based on their unique identifiers.
        /// </summary>
        /// <param name="Ids">The list of unique identifiers of the routes to be deleted.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean value indicating success or failure.</returns>
        Task<bool> DeleteRoutes(List<Guid> Ids);

        /// <summary>
        /// Updates a collection of routes with new data.
        /// </summary>
        /// <param name="routesList">The collection of routes with updated data.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean value indicating success or failure.</returns>
        Task<bool> UpdateRoutes(IEnumerable<RouteDto> routesList);
    }
}

