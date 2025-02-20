using AutoMapper;
using Microsoft.Extensions.Logging;
using MSLogistics.Application.Repositories.IRouteRepository;
using MSLogistics.Application.Repositories.IStopRepository;
using MSLogistics.Application.Repositories.IVehicleRepository;
using MSLogistics.Application.ValueObjects.DTOs.Route;
using MSLogistics.Application.ValueObjects.Enums;
using MSLogistics.Domain;

namespace MSLogistics.Application.Services.RouteService
{
	public class RouteService : IRouteService
	{
        private readonly IRouteRepository _routeRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IStopRepository _stopRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RouteService> _logger;

        public RouteService(IRouteRepository routeRepository, IVehicleRepository vehicleRepository,IStopRepository stopRepository,
            IMapper mapper, ILogger<RouteService> logger)
        {
            _routeRepository = routeRepository;
            _vehicleRepository = vehicleRepository;
            _stopRepository = stopRepository;
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
                // Map RouteDto to Route
                var routeEntities = _mapper.Map<IEnumerable<Route>>(routesList);

                // Get existing stops from the database
                var existingStops = await _stopRepository.GetAllAsync();

                foreach (var route in routeEntities)
                {
                    route.Id = Guid.NewGuid(); // Assign a new ID to the route

                    var vehicle = await _vehicleRepository.GetByIdAsync(route.Vehicle.Id);
                    if (vehicle == null)
                    {
                        _logger.LogWarning($"Vehicle with ID {route.VehicleId} does not exist. Cannot create route.");
                        return false; // Vehicle does not exist
                    }

                    route.VehicleId = vehicle.Id;
                    route.Vehicle = null; // Avoid EF Core tracking issues by clearing navigation property

                    foreach (var stop in route.Stops.ToList())
                    {
                        var existingStop = existingStops.FirstOrDefault(s => s.Id == stop.Id);

                        if (existingStop != null)
                        {
                            // Reuse the tracked stop instance and update its properties if needed
                            stop.RouteId = route.Id;
                            stop.Name = existingStop.Name; // Example: update fields if necessary

                            // Replace the stop with the existing tracked instance
                            route.Stops.Remove(stop);
                            route.Stops.Add(existingStop);
                        }
                        else
                        {
                            // For new stops, set the RouteId
                            stop.RouteId = route.Id;
                        }
                    }
                }

                // Add the routes to the repository
                return await _routeRepository.AddRangeAsync(routeEntities);
            }
            catch (Exception ex)
            {
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception while inserting routes.\n" +
                    $"Exception:\n{ex.Message}\nInner Exception:\n{ex.InnerException}\nStack Trace:\n{ex.StackTrace}");

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
                route => route.Stops) ?? new Route(); 

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
                    // Retrieve the existing route from the database
                    var existingRoute = await _routeRepository.GetByIdAsync(routeDto.Id);

                    if (existingRoute == null)
                    {
                        _logger.LogError((int)LogEventId.DataAccessError, $"Route with ID {routeDto.Id} not found for update.");
                        continue;
                    }

                    // Handle the Vehicle relationship
                    if (routeDto.Vehicle != null)
                    {
                        var newVehicle = await _vehicleRepository.GetByIdAsync(routeDto.Vehicle.Id);

                        if (newVehicle == null)
                        {
                            _logger.LogError((int)LogEventId.DataAccessError, $"Vehicle with ID {routeDto.Vehicle.Id} not found for route {routeDto.Id}.");
                            continue;
                        }

                        existingRoute.VehicleId = newVehicle.Id;
                        existingRoute.Vehicle = newVehicle;
                    }

                    // Handle Stops
                    var updatedStops = new List<Stop>();
                    var existingStops = await _stopRepository.GetAllAsync();

                    foreach (var stopDto in routeDto.Stops)
                    {
                        var existingStop = existingStops.FirstOrDefault(s => s.Id == stopDto.Id);
                        if (existingStop != null)
                        {
                            // Reuse tracked stop instance and update properties
                            _mapper.Map(stopDto, existingStop);
                            updatedStops.Add(existingStop);
                        }
                        else
                        {
                            // Map and add new stop
                            var newStop = _mapper.Map<Stop>(stopDto);
                            newStop.RouteId = existingRoute.Id;
                            updatedStops.Add(newStop);
                        }
                    }

                    // Update the route properties
                    _mapper.Map(routeDto, existingRoute);

                    // Replace the Stops collection with the updated list
                    existingRoute.Stops = updatedStops;

                    routesToUpdate.Add(existingRoute);
                }

                // If there are any routes to update, call UpdateRangeAsync
                if (routesToUpdate.Any())
                {
                    return await _routeRepository.UpdateRangeAsync(routesToUpdate);
                }

                _logger.LogError((int)LogEventId.DataAccessError, "No valid routes found to update.");
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception details with the specified format
                _logger.LogError((int)LogEventId.DataAccessError,
                    $"Exception occurred while updating routes.\n" +
                    $"Exception:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return false;
            }
        }
    }
}

