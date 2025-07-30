namespace SmartDesk.Application.Configurations
{
    public class SmtpSettings
    {
        public string Host { get; set; } = "smtp.example.com";
        public int Port { get; set; } = 587;
        public bool UseSsl { get; set; } = true;
        public string From { get; set; } = "no-reply@example.com";
        public string To { get; set; } = "user@example.com";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
