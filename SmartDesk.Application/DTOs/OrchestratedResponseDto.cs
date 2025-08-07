using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDesk.Application.DTOs
{
    public class OrchestratedResponseDto
    {
        public string Summary { get; set; } = string.Empty;
        public List<ScheduleItemDto> Items { get; set; } = new();
        public List<string> Sources { get; set; } = new List<string>();
    }

}
