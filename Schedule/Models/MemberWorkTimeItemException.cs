using System;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class MemberWorkTimeItemException
    {
        public string MemberWorkTimeItemId { get; set; }
        public DateTimeOffset ExceptionDate { get; set; }
        public Status Status { get; set; }
    }
}