using Microsoft.AspNetCore.Mvc;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using System.Threading.Tasks;

namespace SmartDesk.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/calendar")]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarPlannerService _planner;

        public CalendarController(ICalendarPlannerService planner)
            => _planner = planner;

        /// <summary>
        /// Retrieves busy and free time slots from the user's Google Calendar within a specified time window.
        /// </summary>
        /// <param name="request">The calendar ID and time range to query</param>
        /// <returns>A list of busy and free time slots</returns>
        /// <response code="200">Returns a list of busy and free periods</response>
        /// <response code="400">If the request is invalid</response>
        [HttpPost("calendar/freebusy")]
        [ProducesResponseType(typeof(FreeBusyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("freebusy")]
        public async Task<ActionResult<FreeBusyDto>> GetFreeBusy([FromBody] CalendarFreeBusyRequest request)
        {
            
            var result = await _planner.GetFreeBusyAsync(request);
            return Ok(result);
        }
    }
}
