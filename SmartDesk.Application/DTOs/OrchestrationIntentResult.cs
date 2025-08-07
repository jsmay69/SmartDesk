using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDesk.Application.DTOs
{
    public class OrchestrationIntentResult
    {
        public string Intent { get; set; } = "Unknown"; // e.g. "PlanMyDay"
        public string[] Agents { get; set; } = Array.Empty<string>(); // e.g. ["Tasks", "Calendar"]
    }

}
