using Microsoft.AspNetCore.Mvc;
using MSLogistics.Application.Exceptions;
using MSLogistics.Application.Services.DispatchGroupService;
using MSLogistics.Application.ValueObjects.DTOs.DispatchGroups;
using mslogistiscs_fyp.ValueObjects.Enums;

namespace mslogistiscs_fyp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DispatchGroupController : Controller
    {
        private readonly IDispatchGroupService _dispatchGroupService;
        private readonly ILogger<RouteController> _logger;

        public DispatchGroupController(IDispatchGroupService dispatchGroupService,
            ILogger<RouteController> logger)
        {
            _dispatchGroupService = dispatchGroupService;
            _logger = logger;
        }


        // GET: DispatchGroup/GetDispatchGroups
        [HttpGet]
        [Route("GetDispatchGroups")]
        public async Task<IActionResult> GetDispatchGroups()
        {
            try
            {
                IEnumerable<DispatchGroupDto> dispatchGroups = await _dispatchGroupService.GetDispatchGroups();

                return Ok(dispatchGroups);
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ServerLogEventId.UnknownError,
                    $"Unexpected exception was caught in DispatchGroupController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // GET DispatchGroup/GetDispatchGroupById:34243sdf2-234324fd23-32bdabb (Guid)
        [HttpGet]
        [Route("GetDispatchGroupById")]
        public async Task<IActionResult> GetDispatchGroupById(Guid Id)
        {
            try
            {
                DispatchGroupDto route = await _dispatchGroupService.GetDispatchGroupById(Id);

                return Ok(route);
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ServerLogEventId.UnknownError,
                    $"Unexpected exception was caught in DispatchGroupController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // POST DispatchGroup/AddDispatchGroups:List<DispatchGroupDto>() (Pass the Dto as List)
        [HttpPost]
        [Route("AddDispatchGroups")]
        public async Task<IActionResult> AddDispatchGroups([FromBody] List<DispatchGroupDto> dispatchGroupsToAdd)
        {
            try
            {
                if (dispatchGroupsToAdd.Count <= 0)
                    throw new RequiredInformationMissingException();

                var result = await _dispatchGroupService.AddDispatchGroups(dispatchGroupsToAdd);

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
                    $"Unexpected exception was caught in DispatchGroupController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // PUT DispatchGroup/UpdateDispatchGroups:List<DispatchGroupDto>() (Pass the Dto as List)
        [HttpPut]
        [Route("UpdateDispatchGroups")]
        public async Task<IActionResult> UpdateDispatchGroups([FromBody] List<DispatchGroupDto> dispatchGroupsToUpdate)
        {
            try
            {
                if (dispatchGroupsToUpdate.Count <= 0)
                    throw new RequiredInformationMissingException();

                var result = await _dispatchGroupService.UpdateDispatchGroups(dispatchGroupsToUpdate);

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
                    $"Unexpected exception was caught in DispatchGroupController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }

        // DELETE DispatchGroup/DeleteDispatchGroups:List<Guid>() (Pass the list of selected ids)
        [HttpDelete]
        [Route("DeleteDispatchGroups")]
        public async Task<IActionResult> DeleteDispatchGroups([FromBody] List<Guid> ids)
        {
            try
            {
                if (ids.Count <= 0)
                    throw new RequiredInformationMissingException();

                var result = await _dispatchGroupService.DeleteDispatchGroups(ids);

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
                    $"Unexpected exception was caught in DispatchGroupController.\nException:\n{ex.Message}\nInner exception:\n{ex.InnerException}\nStack trace:\n{ex.StackTrace}");

                return StatusCode(500, "An unknown error occurred on the server.");
            }
        }
    }
}

