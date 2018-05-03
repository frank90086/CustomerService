using System;
using System.Collections.Generic;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class MemberBooking
    {
        public MemberBooking()
        {
            Assigners = new List<MemberBookingAssigner>();
        }

        public string Id { get; set; }
        public string MemberId { get; set; }
        public string SchoolRoomId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Source { get; set; }
        public string SourceKey { get; set; }
        public string SourceKey2 { get; set; }
        public string SourceKey3 { get; set; }
        public string GroupId { get; set; }
        public long Classes { get; set; }
        public BookingStatus Status { get; set; }

        public virtual ICollection<MemberBookingAssigner> Assigners { get; set; }
    }
}