using Microsoft.EntityFrameworkCore;
using SmartDesk.Domain.Common;
using SmartDesk.Infrastructure.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDesk.Infrastructure.Persistence
{
    public class SmartDeskDbContext : DbContext
    {
        private readonly IDomainEventDispatcher _dispatcher;

        public SmartDeskDbContext(
            DbContextOptions<SmartDeskDbContext> options,
            IDomainEventDispatcher dispatcher
        ) : base(options)
        {
            _dispatcher = dispatcher;
        }

        public DbSet<SmartDesk.Domain.Entities.TodoItem> TodoItems => Set<SmartDesk.Domain.Entities.TodoItem>();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 1. Persist to the database
            var result = await base.SaveChangesAsync(cancellationToken);

            // 2. Dispatch any domain events raised
            var entities = ChangeTracker
                .Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.Events.Any())
                .ToList();

            foreach (var entity in entities)
            {
                foreach (var @event in entity.Events)
                {
                    await _dispatcher.DispatchAsync(@event);
                }
                entity.ClearEvents();
            }

            return result;
        }
    }
}
