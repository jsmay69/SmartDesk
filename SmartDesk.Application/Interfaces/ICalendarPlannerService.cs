using SmartDesk.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace SmartDesk.Application.Interfaces
{
    public interface ICalendarPlannerService
    {
        Task<FreeBusyDto> GetFreeBusyAsync(string calendarId, DateTime from, DateTime to);
    }
}
