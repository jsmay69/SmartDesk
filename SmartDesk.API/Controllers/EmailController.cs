using Microsoft.AspNetCore.Mvc;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using System.Threading.Tasks;

namespace SmartDesk.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/email")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSummarizerService _summarizer;

        public EmailController(IEmailSummarizerService summarizer)
        {
            _summarizer = summarizer;
        }

        public class EmailRequest
        {
            public string RawEmailText { get; set; } = string.Empty;
        }

        /// <summary>
        /// Summarize the given email and extract action items.
        /// </summary>
        /// <param name="request">EmailSummarizeRequest</param>
        /// <returns>A summary and a list of extracted tasks</returns>
        /// <response code="200">Returns the email summary and action items</response>
        /// <response code="400">If the request payload is invalid</response>
        [ProducesResponseType(typeof(EmailSummaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("summarize")]
        [Consumes("application/json", "text/plain")]
        public async Task<ActionResult<EmailSummaryDto>> Summarize([FromBody] EmailSummarizeRequest request)
        {
            var result = await _summarizer.SummarizeAsync(request.RawEmailText);
            return Ok(result);
        }
    }
}
