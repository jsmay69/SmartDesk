using SmartDesk.Application.DTOs;
using System.Threading.Tasks;

namespace SmartDesk.Application.Interfaces
{
    public interface IEmailSummarizerService
    {
        Task<EmailSummaryDto> SummarizeAsync(string rawEmailText);
    }
}
