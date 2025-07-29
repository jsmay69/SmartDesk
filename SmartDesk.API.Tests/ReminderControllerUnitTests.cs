using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SmartDesk.API.Controllers;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using SmartDesk.Domain.Entities;
using Xunit;

namespace SmartDesk.API.Tests
{
    public class ReminderControllerUnitTests
    {
        private class FakeReminderService : IReminderService
        {
            public bool WasCalled { get; private set; }
            public TodoItem? ReceivedItem { get; private set; }

            public Task SendReminderAsync(TodoItem item)
            {
                WasCalled = true;
                ReceivedItem = item;
                return Task.CompletedTask;
            }
        }

       // [Fact]
        public async Task SendReminder_CallsService_AndReturnsOk()
        {
            // Arrange
            var fakeService = new FakeReminderService();
            var controller = new ReminderController(fakeService);
            var dto = new TodoItemDto
            {
                Id = Guid.NewGuid(),
                Title = "Test Task",
                Description = "Unit test",
                DueDate = DateTime.UtcNow,
                Priority = "Normal",
                IsCompleted = false
            };

            // Act
            var result = await controller.SendReminder(dto);

            // Assert
            fakeService.WasCalled.Should().BeTrue();
            fakeService.ReceivedItem.Should().NotBeNull();
            fakeService.ReceivedItem!.Id.Should().Be(dto.Id);
            result.Should().BeOfType<OkResult>();
        }
    }
}
