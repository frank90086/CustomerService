using System;
using System.Collections.Generic;
using System.Reflection;
using Omi.Education.Enums;
using Omi.Education.Web.Management.Services.Models;

namespace Omi.Education.Web.Management.Services
{
    public class InitialModel
    {
        public List<MemberWorkTime> Teachers { get; set; }
        public List<MemberBalance> Balances { get; set; }
        public List<Schedule> Schedules { get; set; }
        public List<MemberBooking> Bookings { get; set; }
        public InitialModel()
        {
            Teachers = new List<MemberWorkTime>();
            Balances = new List<MemberBalance>();
            Schedules = new List<Schedule>();
            Bookings = new List<MemberBooking>();
            Initial();
        }

        private void Initial()
        {
            bool isStop = false;
            while (!isStop)
            {
                Teachers.Add(TeacherGenerator());
                if (Teachers.Count > 4)
                    isStop = true;
            }

            Balances.Add(BalanceGenerator());
        }

        private MemberWorkTime TeacherGenerator()
        {
            Random rm = new Random();
            string memberId = PublicMethod.GetToken();

            MemberWorkTime Teacher = new MemberWorkTime()
            {
                MemberId = memberId,
                ReadyTime = rm.Next(0, 13),
                Status = Status.Enabled,
                EffectiveDate = DateTimeOffset.Now
            };

            for (int i = 1; i < 8; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    MemberWorkTimeItem work = new MemberWorkTimeItem()
                    {
                        Id = PublicMethod.GetToken(),
                        MemberId = memberId,
                        Weekday = (DayOfWeek) i,
                        Time = TimeSpan.FromHours(j),
                        EffectiveDate = DateTime.Now
                    };
                    Teacher.Times.Add(work);
                }
            }
            return Teacher;
        }

        private MemberBalance BalanceGenerator()
        {
            string memberId = PublicMethod.GetToken();
            MemberBalance mb = new MemberBalance(){
                Id = PublicMethod.GetToken(),
                MemberId = memberId,
                Type = BalanceType.Point,
                TotalBalances = (decimal)60.00,
                AvailableBalances = (decimal)60.00,
                ExpiredDate = DateTime.Now.AddDays(150)
            };

            return mb;
        }
    }
}