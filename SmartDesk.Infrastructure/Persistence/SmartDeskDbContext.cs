using Microsoft.EntityFrameworkCore;
using SmartDesk.Domain.Entities;

namespace SmartDesk.Infrastructure.Persistence;

public class SmartDeskDbContext : DbContext
{
    public SmartDeskDbContext(DbContextOptions<SmartDeskDbContext> options) : base(options) { }
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}
