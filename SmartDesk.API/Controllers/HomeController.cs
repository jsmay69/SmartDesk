using Microsoft.AspNetCore.Mvc;

namespace SmartDesk.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// Basic health‐check/welcome endpoint.
        /// </summary>
        [HttpGet]
        public ActionResult<string> Get()
            => Ok("Welcome to the SmartDesk Agent API v1.0!");
    }
}
