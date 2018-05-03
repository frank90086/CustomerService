using System;
using System.Collections.Generic;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class AllowTimeTable
    {

        public DayOfWeek WeekDay { get; set; }
        public TimeSpan Time { get; set; }
    }
}