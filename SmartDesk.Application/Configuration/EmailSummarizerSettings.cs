using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDesk.Application.Configuration
{
    public class EmailSummarizerSettings
    {
        public string Provider { get; set; } = "Ollama"; // "OpenAI";
        public string OpenAIApiKey { get; set; } = "";
        public string OpenAIModel { get; set; } = "gpt-4";
        public string OllamaEndpoint { get; set; } = "http://localhost:11434/v1";
        public string OllamaModel { get; set; } = "llama3.2:3b";
    }
}
