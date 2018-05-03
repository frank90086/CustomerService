using System;
using System.Collections.Generic;

namespace Omi.Education.Web.Management.Services.Models
{
    public class MemberWorkTimeItem
    {
        public MemberWorkTimeItem()
        {
            Exceptions = new List<MemberWorkTimeItemException>();
        }
        public string Id { get; set; }
        public string MemberId { get; set; }
        public DayOfWeek Weekday { get; set; }
        public TimeSpan Time { get; set; }
        public int? ContinuousIndex { get; set; }
        public int? ContinuousEnd { get; set; }
        public DateTimeOffset? EffectiveDate { get; set; }

        public ICollection<MemberWorkTimeItemException> Exceptions { get; set; }
    }
}