using System;
using System.Collections.Generic;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class MemberWorkTime
    {
        public MemberWorkTime()
        {
            Times = new List<MemberWorkTimeItem>();
        }
        public string MemberId { get; set; }
        public int? ReadyTime { get; set; }
        public DateTimeOffset? EffectiveDate { get; set; }
        public Status Status { get; set; }

        public ICollection<MemberWorkTimeItem> Times { get; set; }
    }
}