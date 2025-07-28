using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartDesk.Infrastructure.Persistence;
using SmartDesk.Infrastructure.Persistence.Repositories;
using SmartDesk.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace SmartDesk.Application.Tests;

public class TodoItemRepositoryTests
{
    private readonly SmartDeskDbContext _context;
    private readonly TodoItemRepository _repo;

    public TodoItemRepositoryTests()
    {
        var opts = new DbContextOptionsBuilder<SmartDeskDbContext>()
            .UseInMemoryDatabase("TestDb").Options;
        _context = new SmartDeskDbContext(opts);
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
