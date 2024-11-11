using Microsoft.AspNetCore.Mvc;
using MSLogistics.Application.Exceptions;
using MSLogistics.Application.Services.VehicleService;
using MSLogistics.Application.ValueObjects.DTOs.Vehicle;
using mslogistiscs_fyp.ValueObjects.Enums;

namespace mslogistiscs_fyp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<VehicleController> _logger;

        public VehicleController(IVehicleService vehicleService,
            ILogger<VehicleController> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
        }

        // GET: Vehicle/GetVehicles
        [HttpGet]
        [Route("GetVehicles")]
        public async Task<IActionResult> GetVehicles()
        {
            try
            {
                IEnumerable<VehicleDto> vehicles = await _vehicleService.GetVehicles();

                return Ok(vehicles);
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ServerLogEventId.UnknownError,
                    $"Unexpected exception was caught in VehicleController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // GET Vehicle/GetVehiclesById:34243sdf2-234324fd23-32bdabb (Guid)
        [HttpGet]
        [Route("GetVehiclesById")]
        public async Task<IActionResult> GetVehiclesById(Guid Id)
        {
            try
            {
                VehicleDto vehicles = await _vehicleService.GetVehicleById(Id);

                return Ok(vehicles);
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ServerLogEventId.UnknownError,
                    $"Unexpected exception was caught in VehicleController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // POST Vehicle/AddVehicles:List<VehicleDto>() (Pass the Dto as List)
        [HttpPost]
        [Route("AddVehicles")]
        public async Task<IActionResult> AddVehicles([FromBody] List<VehicleDto> vehiclesToAdd)
        {
            try
            {
                if (vehiclesToAdd.Count <= 0)
                    throw new RequiredInformationMissingException();

                var result = await _vehicleService.AddVehicles(vehiclesToAdd);

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
                    $"Unexpected exception was caught in VehicleController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // PUT Vehicle/UpdateVehicles:List<VehicleDto>() (Pass the Dto as List)
        [HttpPut]
        [Route("UpdateVehicles")]
        public async Task<IActionResult> UpdateVehicles([FromBody] List<VehicleDto> vehiclesToUpdate)
        {
            try
            {
                if (vehiclesToUpdate.Count <= 0)
                    throw new RequiredInformationMissingException();

                var result = await _vehicleService.UpdateVehicles(vehiclesToUpdate);

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
                    $"Unexpected exception was caught in VehicleController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // DELETE Vehicle/DeleteVehicles:List<Guid>() (Pass the list of selected ids)
        [HttpDelete]
        [Route("DeleteVehicles")]
        public async Task<IActionResult> DeleteVehicles([FromBody] List<Guid> ids)
        {
            try
            {
                if (ids.Count <= 0)
                    throw new RequiredInformationMissingException();

                var result = await _vehicleService.DeleteVehicles(ids);

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
                    $"Unexpected exception was caught in VehicleController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }
    }
}

