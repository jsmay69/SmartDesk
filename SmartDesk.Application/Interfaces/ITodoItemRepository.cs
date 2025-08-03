using SmartDesk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartDesk.Application.Interfaces;

public interface ITodoItemRepository
{
    Task<List<TodoItem>> GetAllAsync();
    Task<List<TodoItem>> FindAsync(Expression<Func<TodoItem, bool>> predicate);
    Task<TodoItem?> GetByIdAsync(Guid id);
    Task<TodoItem> AddAsync(TodoItem item);
    Task UpdateAsync(TodoItem item);
    Task DeleteAsync(Guid id);
}
