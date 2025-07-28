using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartDesk.Infrastructure.Persistence;
using SmartDesk.Infrastructure.Persistence.Repositories;
using SmartDesk.Domain.Entities;
using SmartDesk.Infrastructure.Common;  
using SmartDesk.Domain.Common;         
using Xunit;

namespace SmartDesk.Application.Tests
{
    public class TodoItemRepositoryTests
    {
        private readonly SmartDeskDbContext _context;
        private readonly TodoItemRepository _repo;

        // No-op dispatcher stub
        private class NoOpDispatcher : IDomainEventDispatcher
        {
            public Task DispatchAsync(IDomainEvent @event)
                => Task.CompletedTask;
        }

        public TodoItemRepositoryTests()
        {
            var opts = new DbContextOptionsBuilder<SmartDeskDbContext>()
                .UseInMemoryDatabase("TestDb").Options;
            // Pass the dispatcher stub into the context
            _context = new SmartDeskDbContext(opts, new NoOpDispatcher());
            _repo = new TodoItemRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistItem()
        {
            var item = new TodoItem { Title = "Test", Priority = "High" };
            var result = await _repo.AddAsync(item);
            result.Id.Should().NotBe(Guid.Empty);
            (await _context.TodoItems.FindAsync(result.Id)).Should().NotBeNull();
        }
    }
}
