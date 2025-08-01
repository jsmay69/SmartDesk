namespace SmartDesk.Application.Configurations
{
    /// <summary>
    /// Settings for selecting and configuring the calendar provider.
    /// </summary>
    public class CalendarSettings
    {
        /// <summary>
        /// Which provider to use: "Google" or "Microsoft"
        /// </summary>
        public string Provider { get; set; } = "Google";

        // --- Google service account JSON path ---
        public string GoogleServiceAccountJsonPath { get; set; } = "/Properties/smartdesk-467518-aaa53c920664.json";

        // --- Microsoft (Azure AD) client credentials ---
        public string MicrosoftTenantId { get; set; } = "";
        public string MicrosoftClientId { get; set; } = "";
        public string MicrosoftClientSecret { get; set; } = "";
    }
}
