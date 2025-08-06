using SmartDesk.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDesk.Application.Interfaces
{
    public interface IAgentOrchestrator
    {
        Task<OrchestratedResponseDto> HandleAsync(string prompt);
    }

}
