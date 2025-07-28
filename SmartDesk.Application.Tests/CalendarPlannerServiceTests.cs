using System;
using System.Threading.Tasks;
using FluentAssertions;
using SmartDesk.Application.Interfaces;
using SmartDesk.Infrastructure.AI;
using Xunit;

namespace SmartDesk.Application.Tests
{
    public class CalendarPlannerServiceTests
    {
        private readonly ICalendarPlannerService _service;

        public CalendarPlannerServiceTests()
        {
            _service = new CalendarPlannerService();
        }

        [Fact]
        public async Task GetFreeBusyAsync_ShouldReturnStubbedSlots()
        {
            // Arrange
            string calendarId = "primary";
            DateTime from = new DateTime(2025, 8, 1, 9, 0, 0, DateTimeKind.Utc);
            DateTime to = new DateTime(2025, 8, 1, 17, 0, 0, DateTimeKind.Utc);

            // Act
            var result = await _service.GetFreeBusyAsync(calendarId, from, to);

            // Assert
            result.CalendarId.Should().Be(calendarId);
            result.BusySlots.Should().HaveCount(1);
            result.FreeSlots.Should().HaveCount(2);

            var busy = result.BusySlots[0];
            busy.Start.Should().Be(from.AddHours((to - from).TotalHours / 3));
            busy.End.Should().Be(busy.Start.AddHours(1));

            result.FreeSlots[0].Start.Should().Be(from);
            result.FreeSlots[0].End.Should().Be(busy.Start);

            result.FreeSlots[1].Start.Should().Be(busy.End);
            result.FreeSlots[1].End.Should().Be(to);
        }
    }
}
