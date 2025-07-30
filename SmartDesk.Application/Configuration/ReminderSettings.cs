namespace SmartDesk.Application.Configurations
{
    /// <summary>
    /// Settings for the background reminder scheduler.
    /// </summary>
    public class ReminderSettings
    {
        /// <summary>
        /// Interval (in seconds) between scheduler polls.
        /// </summary>
        public int PollIntervalSeconds { get; set; } = 60;

        /// <summary>
        /// Minutes before DueDate to send the reminder.
        /// </summary>
        public int LeadTimeMinutes { get; set; } = 15;
    }
}
