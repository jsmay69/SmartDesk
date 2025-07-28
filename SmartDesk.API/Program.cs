using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartDesk.Application.Interfaces;
using SmartDesk.Domain.Entities;
using SmartDesk.Infrastructure.AI;
using SmartDesk.Infrastructure.Persistence;
using SmartDesk.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using SmartDesk.Application.Interfaces;
using SmartDesk.Infrastructure.AI;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<SmartDeskDbContext>(opts =>
    opts.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=smartdesk.db"));

// Dependency Injection
builder.Services.AddScoped<ITodoItemRepository, TodoItemRepository>();
builder.Services.AddScoped<INaturalLanguageTaskParser, TaskParserService>();
builder.Services.AddScoped<IEmailSummarizerService, EmailSummarizerService>();
builder.Services.AddScoped<ICalendarPlannerService, CalendarPlannerService>();

// Controllers
builder.Services.AddControllers();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Migrations & Seed Data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SmartDeskDbContext>();
    db.Database.Migrate();

    if (!db.TodoItems.Any())
    {
        db.TodoItems.AddRange(
            new TodoItem
            {
                Title = "Welcome to SmartDesk",
                Description = "This is your first to-do item.",
                DueDate = DateTime.UtcNow.AddDays(2),
                Priority = "High"
            },
            new TodoItem
            {
                Title = "Explore the API",
                Description = "Try GET /api/v1.0/todoitems and POST /api/v1.0/todoitems/parse",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = "Normal"
            }
        );
        db.SaveChanges();
    }
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
