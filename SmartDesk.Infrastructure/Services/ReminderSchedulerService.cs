using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartDesk.Application.Configurations;
using SmartDesk.Application.Interfaces;
using Prometheus;



namespace SmartDesk.Infrastructure.Services
{
    /// <summary>
    /// Periodically polls for tasks due soon and sends reminders.
    /// </summary>
    public class ReminderSchedulerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReminderSchedulerService> _logger;
        private readonly ReminderSettings _settings;

        private static readonly Counter _remindersSent = Metrics
                    .CreateCounter("smartdesk_reminders_sent_total",
                   "Total number of task reminders sent");

        public ReminderSchedulerService(
            IServiceScopeFactory scopeFactory,
            IOptions<ReminderSettings> opts,
            ILogger<ReminderSchedulerService> logger)
        {
            _scopeFactory = scopeFactory;
            _settings = opts.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReminderScheduler started: polls every {Interval}s, lead time {Lead}m",
                _settings.PollIntervalSeconds, _settings.LeadTimeMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<ITodoItemRepository>();
                    var reminder = scope.ServiceProvider.GetRequiredService<IReminderService>();

                    var now = DateTime.UtcNow;
                    var deadline = now.AddMinutes(_settings.LeadTimeMinutes);

                    // Fetch all tasks and filter in-memory; for large datasets you'd want a dedicated FindAsync
                    var allTasks = await repo.GetAllAsync();
                    var toRemind = allTasks
                        .Where(t => !t.IsReminderSent
                                    && !t.IsCompleted
                                    && t.DueDate.HasValue
                                    && t.DueDate.Value >= now
                                    && t.DueDate.Value <= deadline)
                        .ToList();

                    foreach (var task in toRemind)
                    {
                        try
                        {
                            _logger.LogInformation("Sending reminder for Task {Id} due at {Due}", task.Id, task.DueDate);
                            _remindersSent.Inc();
                            await reminder.SendReminderAsync(task);

                            task.IsReminderSent = true;
                            await repo.UpdateAsync(task);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send reminder for Task {Id}", task.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in ReminderScheduler loop");
                }

                await Task.Delay(TimeSpan.FromSeconds(_settings.PollIntervalSeconds), stoppingToken);
            }

            _logger.LogInformation("ReminderScheduler stopping.");
        }
    }
}
