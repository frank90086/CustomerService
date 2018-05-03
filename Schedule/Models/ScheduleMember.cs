using System;
using System.Collections.Generic;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class ScheduleMember
    {
        public string ScheduleId { get; set; }
        public string MemberId { get; set; }
        public InviteStatus Status { get; set; }
    }
}