using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SmartDesk.API;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using SmartDesk.Domain.Entities;
using Xunit;

namespace SmartDesk.API.Tests
{
    public class ReminderSmokeTests
    {
        private class TestReminderService : IReminderService
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
        public async Task CompletingTodoItem_TriggersReminderService()
        {
            // Arrange
            var testService = new TestReminderService();
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Replace IReminderService with test stub
                        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IReminderService));
                        if (descriptor != null) services.Remove(descriptor);
                        services.AddSingleton<IReminderService>(testService);
                    });
                });
            var client = factory.CreateClient();

            // 1. Create a new TodoItem
            var newItem = new TodoItemDto
            {
                Title = "Smoke Test Task",
                Description = "Test domain event",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = "Normal",
                IsCompleted = false
            };
            var createContent = new StringContent(JsonSerializer.Serialize(newItem), Encoding.UTF8, "application/json");
            var createResponse = await client.PostAsync("/api/v1.0/todoitems", createContent);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = JsonSerializer.Deserialize<TodoItemDto>(
                await createResponse.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            created.Should().NotBeNull();
            created!.IsCompleted.Should().BeFalse();

            // 2. Complete the TodoItem
            created.IsCompleted = true;
            var updateContent = new StringContent(JsonSerializer.Serialize(created), Encoding.UTF8, "application/json");
            var updateResponse = await client.PutAsync($"/api/v1.0/todoitems/{created.Id}", updateContent);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Assert: reminder service was called
            testService.WasCalled.Should().BeTrue();
            testService.ReceivedItem.Should().NotBeNull();
            testService.ReceivedItem!.Id.Should().Be(created.Id);
        }
    }
}
