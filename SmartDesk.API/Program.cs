using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus;
using SmartDesk.API;

using SmartDesk.Application.Configurations;
using SmartDesk.Application.Interfaces;
using SmartDesk.Domain.Common;
using SmartDesk.Domain.Entities;
using SmartDesk.Domain.Events;
using SmartDesk.Infrastructure.Agent;
using SmartDesk.Infrastructure.AI;
using SmartDesk.Infrastructure.Common;
using SmartDesk.Infrastructure.Events;
using SmartDesk.Infrastructure.Persistence;
using SmartDesk.Infrastructure.Persistence.Repositories;
using SmartDesk.Infrastructure.Services;
using System;
using System.Linq;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Database & Repositories 
builder.Services.AddDbContext<SmartDeskDbContext>(opts =>
    opts.UseSqlite(config.GetConnectionString("DefaultConnection")
        ?? "Data Source=smartdesk.db"));
builder.Services.AddScoped<ITodoItemRepository, TodoItemRepository>();

// Natural Language Task Parser 
builder.Services.AddScoped<INaturalLanguageTaskParser, TaskParserService>();

// Domain Events & Notifications 
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<IEventHandler<TodoItemCompletedEvent>, NotifyOnCompletionHandler>();

// Real SMTP Reminder Service
builder.Services.Configure<SmtpSettings>(config.GetSection("Smtp"));
builder.Services.AddScoped<IReminderService, SmtpReminderService>();

// Background Reminder Scheduler
builder.Services.Configure<ReminderSettings>(config.GetSection("Reminder"));
builder.Services.AddHostedService<ReminderSchedulerService>();



builder.Services.Configure<LlmSettings>(builder.Configuration.GetSection("LLM"));

// Natural Language Query Service
builder.Services.AddHttpClient();
builder.Services.AddScoped<OpenAiClient>();
builder.Services.AddScoped<OllamaClient>();

builder.Services.AddScoped<ILLMClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<LlmSettings>>().Value;
    return settings.Provider.Equals("Ollama", StringComparison.OrdinalIgnoreCase)
        ? sp.GetRequiredService<OllamaClient>()
        : sp.GetRequiredService<OpenAiClient>();
});

// Orchestrator Agent
builder.Services.AddScoped<IAgentOrchestrator, AgentOrchestrator>();

// Natural Language Query Agent
builder.Services.AddScoped<IQueryAgentService, QueryAgentService>();

// Calendar Planner {Google & Microsoft) 
builder.Services.Configure<CalendarSettings>(
    builder.Configuration.GetSection("Calendar"));

builder.Services.AddScoped<GoogleCalendarPlannerService>();
builder.Services.AddScoped<MicrosoftCalendarPlannerService>();

builder.Services.AddScoped<ICalendarPlannerService>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<CalendarSettings>>().Value;
    return settings.Provider.Equals("Microsoft", StringComparison.OrdinalIgnoreCase)
        ? sp.GetRequiredService<MicrosoftCalendarPlannerService>()
        : sp.GetRequiredService<GoogleCalendarPlannerService>();
});


builder.Services.AddHttpClient<OpenAIEmailSummarizerService>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<LlmSettings>>().Value;
    client.BaseAddress = new Uri(settings.OpenAI.Endpoint);
});

builder.Services.AddHttpClient<OllamaEmailSummarizerService>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<LlmSettings>>().Value;
    client.BaseAddress = new Uri(settings.Ollama.Endpoint);
});

builder.Services.AddScoped<OpenAIEmailSummarizerService>();
builder.Services.AddScoped<OllamaEmailSummarizerService>();
builder.Services.AddScoped<IEmailSummarizerService>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<LlmSettings>>().Value;
    return settings.Provider.Equals("Ollama", StringComparison.OrdinalIgnoreCase)
        ? sp.GetRequiredService<OllamaEmailSummarizerService>()
        : sp.GetRequiredService<OpenAIEmailSummarizerService>();
});

// Health Checks & Metrics 
builder.Services.AddHealthChecks()
    .AddDbContextCheck<SmartDeskDbContext>("Database")
    .AddCheck<SmtpHealthCheck>("SMTP");

// API Versioning & Swagger 
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
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


var app = builder.Build();

// Migrate & Seed Data 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SmartDeskDbContext>();
    db.Database.Migrate();

    if (!db.TodoItems.Any())
    {
        db.TodoItems.AddRange(
            new TodoItem { Title = "Welcome to SmartDesk", Description = "This is your first to?do item.", DueDate = DateTime.UtcNow.AddDays(2), Priority = "High" },
            new TodoItem { Title = "Explore the API", Description = "Try GET /api/v1.0/todoitems and POST /api/v1.0/todoitems/parse", DueDate = DateTime.UtcNow.AddDays(1), Priority = "Normal" }
        );
        db.SaveChanges();
    }
}

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Expose Prometheus metrics
app.UseMetricServer();
app.UseHttpMetrics();

app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
