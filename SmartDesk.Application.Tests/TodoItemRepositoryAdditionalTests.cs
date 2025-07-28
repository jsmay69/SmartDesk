using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartDesk.Infrastructure.Persistence;
using SmartDesk.Infrastructure.Persistence.Repositories;
using SmartDesk.Domain.Entities;
using SmartDesk.Infrastructure.Common;  
using SmartDesk.Domain.Common;          
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDesk.Application.Tests;

public class TodoItemRepositoryAdditionalTests
{
    private readonly SmartDeskDbContext _context;
    private readonly TodoItemRepository _repo;


    /// <summary>
    /// A stub that does nothing when domain events are dispatched.
    /// </summary>
    private class NoOpDispatcher : IDomainEventDispatcher
    {
        public Task DispatchAsync(IDomainEvent @event)
            => Task.CompletedTask;
    }

    public TodoItemRepositoryAdditionalTests()
    {
        var opts = new DbContextOptionsBuilder<SmartDeskDbContext>()
    .UseInMemoryDatabase("TestDb").Options;
        _context = new SmartDeskDbContext(opts, new NoOpDispatcher());
        _repo = new TodoItemRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllItems()
    {
        var t1 = await _repo.AddAsync(new TodoItem { Title = "T1" });
        var t2 = await _repo.AddAsync(new TodoItem { Title = "T2" });
        var all = await _repo.GetAllAsync();
        all.Should().BeEquivalentTo(new List<TodoItem> { t1, t2 }, opts => opts.Excluding(x => x.CreatedAt));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectOrNull()
    {
        var t = await _repo.AddAsync(new TodoItem { Title = "Only" });
        var found = await _repo.GetByIdAsync(t.Id);
        found.Should().NotBeNull().And.Subject.As<TodoItem>().Title.Should().Be("Only");
        var nf = await _repo.GetByIdAsync(Guid.NewGuid());
        nf.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExisting()
    {
        var t = await _repo.AddAsync(new TodoItem { Title = "Old" });
        t.Title = "New";
        await _repo.UpdateAsync(t);
        var fetched = await _repo.GetByIdAsync(t.Id);
        fetched!.Title.Should().Be("New");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveItem()
    {
        var t = await _repo.AddAsync(new TodoItem { Title = "ToDelete" });
        await _repo.DeleteAsync(t.Id);
        var fetched = await _repo.GetByIdAsync(t.Id);
        fetched.Should().BeNull();
    }
}
