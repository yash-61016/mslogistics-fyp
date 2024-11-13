using Microsoft.AspNetCore.Mvc;
using MSLogistics.Application.Exceptions;
using MSLogistics.Application.Services.StopService;
using MSLogistics.Application.ValueObjects.DTOs.Stop;
using mslogistiscs_fyp.ValueObjects.Enums;

namespace mslogistiscs_fyp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StopController : Controller
    {
        private readonly IStopService _stopService;
        private readonly ILogger<StopController> _logger;

        public StopController(IStopService stopService,
            ILogger<StopController> logger)
        {
            _stopService = stopService;
            _logger = logger;
        }

        // GET: Stop/GetStops
        [HttpGet]
        [Route("GetStops")]
        public async Task<IActionResult> GetStops()
        {
            try
            {
                IEnumerable<StopDto> stops = await _stopService.GetStops();

                return Ok(stops);
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ServerLogEventId.UnknownError,
                    $"Unexpected exception was caught in StopController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // GET Stop/GetStopById:34243sdf2-234324fd23-32bdabb (Guid)
        [HttpGet]
        [Route("GetStopById")]
        public async Task<IActionResult> GetStopById(Guid Id)
        {
            try
            {
                StopDto stop = await _stopService.GetStopById(Id);

                return Ok(stop);
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ServerLogEventId.UnknownError,
                    $"Unexpected exception was caught in StopController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // POST Stop/AddStops:List<StopDto>() (Pass the Dto as List)
        [HttpPost]
        [Route("AddStops")]
        public async Task<IActionResult> AddStops([FromBody] List<StopDto> stopsToAdd)
        {
            try
            {
                if (stopsToAdd.Count <= 0)
                    throw new RequiredInformationMissingException();

                var result = await _stopService.AddStops(stopsToAdd);

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
                    $"Unexpected exception was caught in StopController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // PUT Stop/UpdateStops:List<StopDto>() (Pass the Dto as List)
        [HttpPut]
        [Route("UpdateStops")]
        public async Task<IActionResult> UpdateStops([FromBody] List<StopDto> stopsToUpdate)
        {
            try
            {
                if (stopsToUpdate.Count <= 0)
                    throw new RequiredInformationMissingException();

                var result = await _stopService.UpdateStops(stopsToUpdate);

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
                    $"Unexpected exception was caught in StopController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // DELETE Stop/DeleteStops:List<Guid>() (Pass the list of selected ids)
        [HttpDelete]
        [Route("DeleteVehicles")]
        public async Task<IActionResult> DeleteVehicles([FromBody] List<Guid> ids)
        {
            try
            {
                if (ids.Count <= 0)
                    throw new RequiredInformationMissingException();

                var result = await _stopService.DeleteStops(ids);

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
                    $"Unexpected exception was caught in StopController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }
    }
}

