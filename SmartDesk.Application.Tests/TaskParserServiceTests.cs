using System;
using System.Threading.Tasks;
using FluentAssertions;
using SmartDesk.Application.Interfaces;
using SmartDesk.Infrastructure.AI;
using Xunit;

namespace SmartDesk.Application.Tests
{
    public class TaskParserServiceTests
    {
        private readonly INaturalLanguageTaskParser _parser;

        public TaskParserServiceTests()
        {
            _parser = new TaskParserService();
        }

        [Theory]
        [InlineData("Do this this morning", 9)]
        [InlineData("Finish report this afternoon", 15)]
        public async Task ParseAsync_ShouldSetSameDay_ForThisMorningOrAfternoon(string input, int expectedHour)
        {
            // Act
            var result = await _parser.ParseAsync(input);

            // Assert: same day at expectedHour
            result.DueDate.Should().NotBeNull();
            var due = result.DueDate.Value;
            due.Date.Should().Be(DateTime.Now.Date);
            due.Hour.Should().Be(expectedHour);
        }

        [Theory]
        [InlineData("Call John tomorrow", 9)]
        [InlineData("Plan tomorrow afternoon", 15)]
        public async Task ParseAsync_ShouldSetNextDay_ForTomorrowPhrases(string input, int expectedHour)
        {
            // Act
            var result = await _parser.ParseAsync(input);

            // Assert: next day at expectedHour
            result.DueDate.Should().NotBeNull();
            var due = result.DueDate.Value;
            due.Date.Should().Be(DateTime.Now.Date.AddDays(1));
            due.Hour.Should().Be(expectedHour);
        }

        [Fact]
        public async Task ParseAsync_ShouldHandleExplicitAtTime_RollToTomorrowIfPast()
        {
            // Arrange: choose a time that’s already past (e.g., midnight)
            var input = "Meeting at 00:00";
            var before = DateTime.Now;

            // Act
            var result = await _parser.ParseAsync(input);

            // Assert
            result.DueDate.Should().NotBeNull();
            var due = result.DueDate.Value;
            // 00:00 => hour 0, so candidate at 00:00 today; since now > midnight, should be tomorrow
            due.Date.Should().Be(before.Date.AddDays(1));
            due.Hour.Should().Be(0);
        }

        [Fact]
        public async Task ParseAsync_DefaultsToNextDay9am_WhenNoKeywords()
        {
            // Act
            var result = await _parser.ParseAsync("Generic task");

            // Assert: next day at 9AM
            var due = result.DueDate.Value;
            due.Date.Should().Be(DateTime.Now.Date.AddDays(1));
            due.Hour.Should().Be(9);
        }
    }
}
