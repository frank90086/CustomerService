using System;
using System.Collections.Generic;
using Omi.Education.Library.SignalR.Connection;
using Omi.Education.Web.Management.Services.Models;

namespace Omi.Education.Web.Management.Services
{
    public interface IScheduleService
    {
        List<AllowTimeTable> GetAllowTime();
        List<AllowTimeTable> GetNotAllowed();
        void CreateBooking(List<DateTimeOffset> selections, out bool isPair);
        void CreateSchedule(MemberBooking model);
        void MakePair(MemberBooking model, out bool isSuccess);
        void ResetModel();
    }
}