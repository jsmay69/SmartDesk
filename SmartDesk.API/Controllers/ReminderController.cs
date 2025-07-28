using Microsoft.AspNetCore.Mvc;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using System.Threading.Tasks;

namespace SmartDesk.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/reminders")]
    public class ReminderController : ControllerBase
    {
        private readonly IReminderService _reminderService;

        public ReminderController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        /// <summary>
        /// Sends a reminder for the given Todo item.
        /// </summary>
        [HttpPost("send")]
        public async Task<IActionResult> SendReminder([FromBody] TodoItemDto item)
        {
            // Convert DTO back to entity (or you could overload the service to accept the DTO directly)
            var entity = new SmartDesk.Domain.Entities.TodoItem
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                DueDate = item.DueDate,
                Priority = item.Priority,
                IsCompleted = item.IsCompleted,
                CreatedAt = System.DateTime.UtcNow
            };

            await _reminderService.SendReminderAsync(entity);
            return Ok();
        }
    }
}
