using SmartDesk.Application.Interfaces;
using SmartDesk.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace SmartDesk.Infrastructure.Services
{
    /// <summary>
    /// Stub that “sends” a reminder by writing to the console.
    /// </summary>
    public class ReminderServiceStub : IReminderService
    {
        public Task SendReminderAsync(TodoItem item)
        {
            Console.WriteLine($"[ReminderService] Reminder for task '{item.Title}' (ID: {item.Id})");
            return Task.CompletedTask;
        }
    }
}
