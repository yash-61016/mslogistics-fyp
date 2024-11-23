using Microsoft.AspNetCore.Mvc;
using MSLogistics.Application.Exceptions;
using MSLogistics.Application.Services.RouteService;
using MSLogistics.Application.ValueObjects.DTOs.Route;
using mslogistiscs_fyp.ValueObjects.Enums;

namespace mslogistiscs_fyp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RouteController : Controller
    {
        private readonly IRouteService _routeService;
        private readonly ILogger<RouteController> _logger;

        public RouteController(IRouteService routeService,
            ILogger<RouteController> logger)
        {
            _routeService = routeService;
            _logger = logger;
        }

        // GET: Route/GetRoutes
        [HttpGet]
        [Route("GetRoutes")]
        public async Task<IActionResult> GetRoutes()
        {
            try
            {
                IEnumerable<RouteDto> stops = await _routeService.GetRoutes();

                return Ok(stops);
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ServerLogEventId.UnknownError,
                    $"Unexpected exception was caught in RouteController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // GET Route/GetRouteById:34243sdf2-234324fd23-32bdabb (Guid)
        [HttpGet]
        [Route("GetRouteById")]
        public async Task<IActionResult> GetRouteById(Guid Id)
        {
            try
            {
                RouteDto route = await _routeService.GetRouteById(Id);

                return Ok(route);
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ServerLogEventId.UnknownError,
                    $"Unexpected exception was caught in RouteController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // POST Route/AddRoutes:List<RouteDto>() (Pass the Dto as List)
        [HttpPost]
        [Route("AddRoutes")]
        public async Task<IActionResult> AddRoutes([FromBody] List<RouteDto> routesToAdd)
        {
            try
            {
                if (routesToAdd.Count <= 0)
                    throw new RequiredInformationMissingException();

                var result = await _routeService.AddRoutes(routesToAdd);

                return Ok(result);
            }
            catch (RequiredInformationMissingException)
            {
                return StatusCode(400, "Required information is missing. Please check your request.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    (int)ServerLogEventId.UnknownError,
                    $"Unexpected exception was caught in RouteController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // PUT Route/UpdateRoutes:List<RouteDto>() (Pass the Dto as List)
        [HttpPut]
        [Route("UpdateRoutes")]
        public async Task<IActionResult> UpdateRoutes([FromBody] List<RouteDto> routesToUpdate)
        {
            try
            {
                if (routesToUpdate.Count <= 0)
                    throw new RequiredInformationMissingException();

                var result = await _routeService.UpdateRoutes(routesToUpdate);

                return Ok(result);
            }
            catch (RequiredInformationMissingException)
            {
                return StatusCode(400, "Required information is missing. Please check your request.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    (int)ServerLogEventId.UnknownError,
                    $"Unexpected exception was caught in RouteController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // DELETE Route/DeleteRoutes:List<Guid>() (Pass the list of selected ids)
        [HttpDelete]
        [Route("DeleteRoutes")]
        public async Task<IActionResult> DeleteRoutes([FromBody] List<Guid> ids)
        {
            try
            {
                if (ids.Count <= 0)
                    throw new RequiredInformationMissingException();

                var result = await _routeService.DeleteRoutes(ids);

                return Ok(result);
            }
            catch (RequiredInformationMissingException)
            {
                return StatusCode(400, "Required information is missing. Please check your request.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    (int)ServerLogEventId.UnknownError,
                    $"Unexpected exception was caught in RouteController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }
    }
}

