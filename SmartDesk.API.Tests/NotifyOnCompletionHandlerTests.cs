using System;
using System.Threading.Tasks;
using FluentAssertions;
using SmartDesk.Infrastructure.Events;
using SmartDesk.Domain.Events;
using SmartDesk.Application.Interfaces;
using SmartDesk.Domain.Entities;
using Xunit;

namespace SmartDesk.API.Tests
{
    public class NotifyOnCompletionHandlerTests
    {
        private class FakeReminderService : IReminderService
        {
            public bool WasCalled { get; private set; }
            public TodoItem? ItemReceived { get; private set; }

            public Task SendReminderAsync(TodoItem item)
            {
                WasCalled = true;
                ItemReceived = item;
                return Task.CompletedTask;
            }
        }

       // [Fact]
        public async Task HandleAsync_ShouldCallReminderService()
        {
            // Arrange
            var fake = new FakeReminderService();
            var handler = new NotifyOnCompletionHandler(fake);
            var todo = new TodoItem
            {
                Id = Guid.NewGuid(),
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow,
                Priority = "Normal",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };
            var @event = new TodoItemCompletedEvent(todo);

            // Act
            await handler.HandleAsync(@event);

            // Assert
            fake.WasCalled.Should().BeTrue();
            fake.ItemReceived.Should().BeSameAs(todo);
        }
    }
}
