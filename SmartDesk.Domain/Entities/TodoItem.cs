using SmartDesk.Domain.Common;
using SmartDesk.Domain.Events;
using System;

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

        public void MarkCompleted()
        {
            if (!IsCompleted)
            {
                IsCompleted = true;
                // Raise a domain event when completed
                RaiseEvent(new TodoItemCompletedEvent(this));
            }
        }
    }
}
