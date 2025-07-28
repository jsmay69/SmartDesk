using System.Threading.Tasks;

namespace SmartDesk.Domain.Common
{
    /// <summary>
    /// Handler for a specific domain event type.
    /// </summary>
    public interface IEventHandler<TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent @event);
    }
}
