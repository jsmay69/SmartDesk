using System.Collections.Generic;

namespace SmartDesk.Domain.Common
{
    /// <summary>
    /// Base class for entities that can raise domain events.
    /// </summary>
    public abstract class BaseEntity
    {
        private readonly List<IDomainEvent> _events = new();
        public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

        protected void RaiseEvent(IDomainEvent @event) => _events.Add(@event);
        public void ClearEvents() => _events.Clear();
    }
}
