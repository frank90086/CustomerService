using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Omi.Education.Enums;
using Omi.Education.Web.Management.Services.Models;

namespace Omi.Education.Web.Management.Services
{
    public class InitialModel
    {
        public List<MemberWorkTime> Teachers { get; set; }
        public List<MemberBalance> Balances { get; set; }
        public List<MemberBooking> Bookings { get; set; }
        public Schedule Schedule { get; set; }
        public List<AvailableDateStock> AvailableDateStock { get; set; }
        public InitialModel()
        {
            Teachers = new List<MemberWorkTime>();
            Balances = new List<MemberBalance>();
            Bookings = new List<MemberBooking>();
            AvailableDateStock = new List<AvailableDateStock>();
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
            StockGenerator();
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

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    if (rm.Next(0, 3) == 2)
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
            }
            return Teacher;
        }

        private void StockGenerator(){
            int dayCount = 0;
            while (dayCount < 30)
            {
                int hourCount = 0;
                DateTimeOffset now = DateTimeOffset.Now.Date;
                while (hourCount < 24)
                {
                    AvailableDateStock stock = new AvailableDateStock()
                    {
                        AvailableDate = now.AddDays(dayCount).AddHours(hourCount),
                        Type = TimeStockType.Course
                    };
                    foreach (MemberWorkTime teacher in Teachers)
                    {
                        MemberWorkTimeItem match = teacher.Times.Where(x => x.Weekday == stock.AvailableDate.DayOfWeek && x.Time.Hours == stock.AvailableDate.Hour).FirstOrDefault();
                        if (match != null)
                        {
                            stock.Members.Add(new AvailableDateStockMember()
                            {
                                AvailableDate = stock.AvailableDate,
                                    Type = TimeStockType.Course,
                                    MemberId = teacher.MemberId
                            });
                        }
                    }
                    AvailableDateStock.Add(stock);
                    hourCount++;
                }
                dayCount++;
            }
        }

        private MemberBalance BalanceGenerator()
        {
            string memberId = PublicMethod.GetToken();
            MemberBalance mb = new MemberBalance()
            {
                Id = PublicMethod.GetToken(),
                MemberId = memberId,
                Type = BalanceType.Point,
                TotalBalances = (decimal) 60.00,
                AvailableBalances = (decimal) 60.00,
                ExpiredDate = DateTime.Now.AddDays(150)
            };

            Schedule = new Schedule()
            {
                Id = PublicMethod.GetToken(),
                MemberId = memberId,
                Name = "Chinese Class",
                Type = ScheduleType.Class,
                Color = "#F4A460",
                Visibled = true,
                IsSystem = false
            };
            return mb;
        }
    }
}