using SmartDesk.Domain.Common;
using SmartDesk.Domain.Entities;
using System;

namespace SmartDesk.Domain.Events
{
    public class TodoItemCompletedEvent : IDomainEvent
    {
        public TodoItem Item { get; }
        public DateTime OccurredAt { get; } = DateTime.UtcNow;

        public TodoItemCompletedEvent(TodoItem item) => Item = item;
    }
}
