using System;
using System.Collections.Generic;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class ScheduleEvent
    {
        public ScheduleEvent()
        {
            Members = new List<ScheduleEventMember>();
            Values = new List<ScheduleEventValue>();
        }

        public string ScheduleId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public ScheduleEventType Type { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Source { get; set; }
        public string SourceKey { get; set; }
        public string SourceKey2 { get; set; }
        public string SourceKey3 { get; set; }
        public string Place { get; set; }
        public ScheduleEventStatus Status { get; set; }

        public virtual ICollection<ScheduleEventMember> Members { get; set; }
        public virtual ICollection<ScheduleEventValue> Values { get; set; }
    }
}