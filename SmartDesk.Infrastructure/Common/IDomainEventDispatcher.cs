using SmartDesk.Domain.Common;
using System.Threading.Tasks;

namespace SmartDesk.Infrastructure.Common
{
    /// <summary>
    /// Dispatches domain events to their handlers.
    /// </summary>
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IDomainEvent @event);
    }
}
