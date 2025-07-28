using SmartDesk.Application.Interfaces;
using SmartDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDesk.Infrastructure.Persistence.Repositories;

public class TodoItemRepository : ITodoItemRepository
{
    private readonly SmartDeskDbContext _context;
    public TodoItemRepository(SmartDeskDbContext context) { _context = context; }

    public async Task<List<TodoItem>> GetAllAsync() => await _context.TodoItems.ToListAsync();
    public async Task<TodoItem?> GetByIdAsync(Guid id) => await _context.TodoItems.FindAsync(id);
    public async Task<TodoItem> AddAsync(TodoItem item) { _context.TodoItems.Add(item); await _context.SaveChangesAsync(); return item; }
    public async Task UpdateAsync(TodoItem item) { _context.TodoItems.Update(item); await _context.SaveChangesAsync(); }
    public async Task DeleteAsync(Guid id) { var t = await _context.TodoItems.FindAsync(id); if (t != null) { _context.TodoItems.Remove(t); await _context.SaveChangesAsync(); } }
}
