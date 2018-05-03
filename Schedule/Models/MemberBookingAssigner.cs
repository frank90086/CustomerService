using System;
using System.Collections.Generic;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class MemberBookingAssigner
    {

        public string Id { get; set; }
        public string MemberBookingId { get; set; }
        public string MemberId { get; set; }
        public AssignerRole Role { get; set; }
        public AssignStatus Status { get; set; }
    }
}