using SmartDesk.Application.Interfaces;            
using SmartDesk.Domain.Common;
using SmartDesk.Domain.Events;
using System.Threading.Tasks;

namespace SmartDesk.Infrastructure.Events
{
    public class NotifyOnCompletionHandler : IEventHandler<TodoItemCompletedEvent>
    {
        private readonly IReminderService _reminder;

        public NotifyOnCompletionHandler(IReminderService reminder)
        {
            _reminder = reminder;
        }

        public Task HandleAsync(TodoItemCompletedEvent @event)
        {
            // Delegate to your reminder service
            return _reminder.SendReminderAsync(@event.Item);
        }
    }
}
