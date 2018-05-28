using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Omi.Education.Library.SignalR.Connection;
using Omi.Education.Web.Management.Services.Models;

namespace Omi.Education.Web.Management.Services
{
    public interface IScheduleService
    {
        InitialModel Model { get; set; }
        List<AllowTimeTableViewModel> GetAllowTime(TimeZoneInfo timezone);
        List<AllowTimeTableViewModel> GetNotAllowed();
        List<ScheduleViewModel> GetSchedule(TimeZoneInfo timezone);
        List<SelectListItem> GetTeacherList();
        void CreateBooking(List<SelectionsViewModel> selections, out bool isPair);
        void CreateSchedule(MemberBooking model);
        void CheckException(string id, SelectionsViewModel selection, out bool hasClass);
        void CreateException(string id, SelectionsViewModel selection, out bool isFinish);
        void CancelException(string id, SelectionsViewModel selection, out bool isFinish);
        void MakePair(MemberBooking model, out bool isSuccess);
        void ResetModel();
    }
}