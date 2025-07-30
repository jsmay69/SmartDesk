using SmartDesk.Domain.Common;
using SmartDesk.Domain.Events;
namespace SmartDesk.Domain.Entities
{
    public class TodoItem : BaseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = "Normal";
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsReminderSent { get; set; } = false;

        public void MarkCompleted()
        {
            if (!IsCompleted)
            {
                IsCompleted = true;
                RaiseEvent(new TodoItemCompletedEvent(this));
            }
        }
    }
}
