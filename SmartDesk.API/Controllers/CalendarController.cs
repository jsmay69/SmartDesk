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
        /// Returns free/busy slots for a given calendar between `From` and `To`.
        /// </summary>
        [HttpPost("freebusy")]
        public async Task<ActionResult<FreeBusyDto>> GetFreeBusy([FromBody] CalendarFreeBusyRequest request)
        {
            
            var result = await _planner.GetFreeBusyAsync(request);
            return Ok(result);
        }
    }
}
