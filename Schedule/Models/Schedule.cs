using System;
using System.Collections.Generic;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class Schedule
    {
        public Schedule()
        {
            Events = new List<ScheduleEvent>();
            Members = new List<ScheduleMember>();
        }

        public string MemberId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public ScheduleType Type { get; set; }
        public string Color { get; set; }
        public bool Visibled { get; set; }
        public bool IsSystem { get; set; }

        public virtual ICollection<ScheduleEvent> Events { get; set; }
        public virtual ICollection<ScheduleMember> Members { get; set; }
    }
}