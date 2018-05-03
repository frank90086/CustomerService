using System;
using System.Collections.Generic;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class MemberProfile
    {
        public MemberProfile()
        {
            Schedules = new List<Schedule>();
            FavoriteTeachers = new List<MemberFavoriteTeacher>();
            CourseBookings = new List<MemberBooking>();
        }
        public string MemberId { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<MemberFavoriteTeacher> FavoriteTeachers { get; set; }
        public virtual ICollection<MemberBooking> CourseBookings { get; set; }
    }
}