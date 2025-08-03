namespace SmartDesk.Application.Configurations
{
    public class LlmSettings
    {
        public string Provider { get; set; } = string.Empty;
        public OpenAiSettings OpenAI { get; set; } = new OpenAiSettings();
        public OllamaSettings Ollama { get; set; } = new OllamaSettings();
    }

    public class OpenAiSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;    
        public string Model { get; set; } = string.Empty;
    }

    public class OllamaSettings
    {
        public string Endpoint { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;   
    }
}