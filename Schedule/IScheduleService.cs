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
        void MakePair(List<DateTime> selections, out bool isPair);
    }
}