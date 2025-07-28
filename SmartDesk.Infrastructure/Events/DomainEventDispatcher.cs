using Microsoft.Extensions.DependencyInjection;
using SmartDesk.Domain.Common;
using SmartDesk.Infrastructure.Common;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDesk.Infrastructure.Events
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _provider;
        public DomainEventDispatcher(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task DispatchAsync(IDomainEvent @event)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
            var handlers = _provider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod("HandleAsync");
                if (method != null)
                {
                    var task = (Task)method.Invoke(handler, new[] { @event });
                    await task;
                }
            }
        }
    }
}
