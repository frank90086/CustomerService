using System;
using System.Collections.Generic;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class AllowTimeTableViewModel
    {
        private readonly TimeZoneInfo _customerTimezone;
        private DateTimeOffset _time;
        public AllowTimeTableViewModel(TimeZoneInfo timezone)
        {
            _customerTimezone = timezone;
        }
        public int Quantity { get; set; }
        public DateTimeOffset Time { get { return _time; } set { _time = TimeZoneInfo.ConvertTime(value, _customerTimezone); } }
    }
}