using System;
using System.Collections.Generic;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class SelectionsViewModel
    {
        private readonly TimeZoneInfo _system;
        private DateTimeOffset _value;
        public SelectionsViewModel()
        {
            _system = TimeZoneInfo.Local;
        }
        public DateTimeOffset SelectTime { get { return _value; } set { _value = TimeZoneInfo.ConvertTime(value, _system); } }
    }
}