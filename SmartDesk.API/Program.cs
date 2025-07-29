using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SmartDesk.API;
using SmartDesk.Application.Configuration;
using SmartDesk.Application.Interfaces;
using SmartDesk.Domain.Common;
using SmartDesk.Domain.Entities;
using SmartDesk.Domain.Events;
using SmartDesk.Infrastructure.AI;
using SmartDesk.Infrastructure.Common;
using SmartDesk.Infrastructure.Events;
using SmartDesk.Infrastructure.Persistence;
using SmartDesk.Infrastructure.Persistence.Repositories;
using SmartDesk.Infrastructure.Services;
using System;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// ??????????? Database & Repositories ???????????
builder.Services.AddDbContext<SmartDeskDbContext>(opts =>
    opts.UseSqlite(config.GetConnectionString("DefaultConnection")
        ?? "Data Source=smartdesk.db"));
builder.Services.AddScoped<ITodoItemRepository, TodoItemRepository>();

// ??????????? Natural?Language Task Parser ???????????
builder.Services.AddScoped<INaturalLanguageTaskParser, TaskParserService>();

// ??????????? Domain Events & Reminders ???????????
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<IEventHandler<TodoItemCompletedEvent>, NotifyOnCompletionHandler>();
builder.Services.AddScoped<IReminderService, ReminderServiceStub>();

// ??????????? Email Summarizer Configuration ???????????
builder.Services.Configure<EmailSummarizerSettings>(
    config.GetSection("EmailSummarizer"));

// Register both implementations
builder.Services.AddScoped<OpenAIEmailSummarizerService>();
builder.Services.AddScoped<OllamaEmailSummarizerService>();

// OpenAI client
builder.Services.AddHttpClient<OpenAIEmailSummarizerService>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com");
});

// Ollama client (uses configured endpoint)
builder.Services.AddHttpClient<OllamaEmailSummarizerService>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<EmailSummarizerSettings>>().Value;
    client.BaseAddress = new Uri(settings.OllamaEndpoint);
});


// Factory to pick provider
builder.Services.AddScoped<IEmailSummarizerService>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<EmailSummarizerSettings>>().Value;
    return settings.Provider.Equals("Ollama", StringComparison.OrdinalIgnoreCase)
        ? sp.GetRequiredService<OllamaEmailSummarizerService>()
        : sp.GetRequiredService<OpenAIEmailSummarizerService>();
});

// ??????????? API Versioning & Swagger ???????????
builder.Services.AddControllers();
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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ??????????? Migrations & Seed Data ???????????
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SmartDeskDbContext>();
    db.Database.Migrate();

    if (!db.TodoItems.Any())
    {
        db.TodoItems.AddRange(
            new TodoItem { Title = "Welcome to SmartDesk", Description = "This is your first to-do item.", DueDate = DateTime.UtcNow.AddDays(2), Priority = "High" },
            new TodoItem { Title = "Explore the API", Description = "Try GET /api/v1.0/todoitems and POST /api/v1.0/todoitems/parse", DueDate = DateTime.UtcNow.AddDays(1), Priority = "Normal" }
        );
        db.SaveChanges();
    }
}

// ??????????? Middleware ???????????
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
