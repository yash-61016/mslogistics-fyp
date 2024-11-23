using AutoMapper;
using Microsoft.Extensions.Logging;
using MSLogistics.Application.Repositories.IRouteRepository;
using MSLogistics.Application.ValueObjects.DTOs.Route;
using MSLogistics.Application.ValueObjects.Enums;
using MSLogistics.Domain;

namespace MSLogistics.Application.Services.RouteService
{
	public class RouteService : IRouteService
	{
        private readonly IRouteRepository _routeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RouteService> _logger;

        public RouteService(IRouteRepository routeRepository,
            IMapper mapper, ILogger<RouteService> logger)
        {
            _routeRepository = routeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> AddRoutes(IEnumerable<RouteDto> routesList)
        {
            if (routesList == null || !routesList.Any())
            {
                return false;
            }

            try
            {
                // Map RouteDto to Stop
                var routeEntities = _mapper.Map<IEnumerable<Route>>(routesList);

                // Assign new IDs to each stop entity
                foreach (var route in routeEntities)
                {
                    route.Id = Guid.NewGuid();
                }

                // Attempt to add stops to the repository
                return await _routeRepository.AddRangeAsync(routeEntities);

            }
            catch (Exception ex)
            {
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception was thrown while inserting a range of records of the {typeof(Route)} type.\n" +
                    $"Exception:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return false;
            }
        }

        public async Task<bool> DeleteRoutes(List<Guid> Ids)
        {
            if (Ids == null || !Ids.Any())
            {
                return false;
            }

            try
            {
                ICollection<Guid> routeIds = new List<Guid>();

                foreach (Guid Id in Ids)
                {
                    Route? route = await _routeRepository.GetByIdAsync(Id);

                    if (route != null)
                        routeIds.Add(route.Id);
                    else
                        _logger.LogError((int)LogEventId.DataAccessError, $"Route with ID {Id} not found in the repository.");
                }

                // Attempt to delete the stops with the specified IDs
                return await _routeRepository.DeleteRangeAsync(routeIds);
            }
            catch (Exception ex)
            {
                // Log the exception details with specified format
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception was thrown while deleting a range of records of the {typeof(Route)} type.\n" +
                    $"Exception:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return false;
            }
        }

        public async Task<RouteDto> GetRouteById(Guid Id)
        {
            Route route = await _routeRepository.GetRoutesByIdWithIncludesAsync(Id,
                route => route.Vehicle,
                route => route.Stops) ?? new Route(); ;

            // Map the routes to RouteDtos
            RouteDto routeDtos = _mapper.Map<RouteDto>(route);

            // Ensure a valid list is returned even if mapping results in null
            return routeDtos ?? new RouteDto();
        }

        public async Task<IEnumerable<RouteDto>> GetRoutes()
        {
            // Retrieve all routes with their associated Vehicles and Stops
            IEnumerable<Route> routes = await _routeRepository.GetAllWithIncludesAsync(
                route => route.Vehicle,
                route => route.Stops) ?? new List<Route>();

            // Map the routes to RouteDtos
            IEnumerable<RouteDto> routeDtos = _mapper.Map<IEnumerable<RouteDto>>(routes);

            // Ensure a valid list is returned even if mapping results in null
            return routeDtos ?? new List<RouteDto>();
        }

        public async Task<bool> UpdateRoutes(IEnumerable<RouteDto> routesList)
        {
            if (routesList == null || !routesList.Any())
            {
                return false;
            }

            try
            {
                var routesToUpdate = new List<Route>();

                foreach (var routeDto in routesList)
                {
                    // Retrieve the existing stop by ID
                    var existingRoute = await _routeRepository.GetByIdAsync(routeDto.Id);

                    if (existingRoute == null)
                    {
                        _logger.LogError((int)LogEventId.DataAccessError, $"Failed to retrive existing route for updating from database.");
                        continue;
                    }

                    // Map the new values onto the existing entity
                    _mapper.Map(routeDto, existingRoute);
                    routesToUpdate.Add(existingRoute);
                }

                // If there are any vehicles to update, call UpdateRangeAsync
                if (routesToUpdate.Any())
                {
                    return await _routeRepository.UpdateRangeAsync(routesToUpdate);
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
                    $"Exception was thrown while updating a range of records of the {typeof(Route)} type.\n" +
                    $"Exception:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return false;
            }
        }
    }
}

