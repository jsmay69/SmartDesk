using SmartDesk.Domain.Entities;
using System.Threading.Tasks;

namespace SmartDesk.Application.Interfaces
{
    /// <summary>
    /// Sends reminders for completed tasks.
    /// </summary>
    public interface IReminderService
    {
        Task SendReminderAsync(TodoItem item);
    }
}
