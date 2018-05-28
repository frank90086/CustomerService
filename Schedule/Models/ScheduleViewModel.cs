using System;
using System.Collections.Generic;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class ScheduleViewModel
    {
        private readonly TimeZoneInfo _customerTimezone;
        private DateTimeOffset _value;
        public ScheduleViewModel(TimeZoneInfo timezone)
        {
            _customerTimezone = timezone;
        }

        public string Name { get; set; }
        public ScheduleType Type { get; set; }
        public string Color { get; set; }
        public bool Visibled { get; set; }
        public bool IsSystem { get; set; }
        public DateTimeOffset StartDate { get { return _value; } set { _value = TimeZoneInfo.ConvertTime(value, _customerTimezone); } }
        public DateTimeOffset EndDate { get { return _value.AddHours(1); } private set { } }
    }
}