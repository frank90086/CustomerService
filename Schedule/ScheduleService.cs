using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Omi.Education.Enums;
using Omi.Education.Enums.Service;
using Omi.Education.Web.Management.Services.Models;

namespace Omi.Education.Web.Management.Services
{
    public class ScheduleService : IScheduleService
    {
        public InitialModel Model { get; set; }
        public ScheduleService()
        {
            Model = new InitialModel();
        }

        public List<AllowTimeTable> GetAllowTime()
        {
            List<MemberWorkTimeItem> list = new List<MemberWorkTimeItem>();
            var worklist = Model.Teachers.Select(x => x.Times.ToList());
            foreach (var item in worklist)
                list.AddRange(item);

            var returnList = list.Distinct(new PropertyCompare<MemberWorkTimeItem>("Weekday", "Time"))
                .Select(x => new AllowTimeTable { WeekDay = x.Weekday, Time = x.Time }).OrderBy(x => x.WeekDay).ThenBy(x => x.Time).ToList();
            return returnList;
        }

        public List<AllowTimeTable> GetNotAllowed()
        {
            List<AllowTimeTable> returnList = new List<AllowTimeTable>();
            var allowList = GetAllowTime();
            for (int i = 1; i <= 7; i++)
            {
                for (int j = 1; j <= 24; j++)
                {
                    var isExist = allowList.Where(x => x.WeekDay == (DayOfWeek) i && x.Time == TimeSpan.FromHours(j)).FirstOrDefault();
                    if (isExist != null)
                        continue;

                    returnList.Add(new AllowTimeTable() { WeekDay = (DayOfWeek) i, Time = TimeSpan.FromHours(j) });
                }
            }
            return returnList;
        }

        public void MakePair(List<DateTime> selections, out bool isPair)
        {
            MemberBalance balance = Model.Balances.FirstOrDefault();
            List<MemberBooking> bookingList = new List<MemberBooking>();
            long classes = (long) Model.Bookings.Where(x => x.MemberId == balance.MemberId).ToList().Count;
            if (selections.Count > 0 && selections.Count <= balance.AvailableBalances)
            {
                foreach (DateTime item in selections)
                {
                    bookingList.Add(new MemberBooking(){
                        Id = PublicMethod.GetToken(),
                        MemberId = balance.MemberId,
                        StartDate = item.ToLocalDateTimeOffset("China Standard Time"),
                        EndDate = item.AddHours(1).ToLocalDateTimeOffset("China Standard Time"),
                        Classes = classes == 0 ? 1 : classes++,
                        Status = BookingStatus.Booking
                    });
                }
                Model.Bookings.AddRange(bookingList);
                balance.AvailableBalances -= bookingList.Count;
                isPair = true;
            }
            else
                isPair = false;
        }
    }
}