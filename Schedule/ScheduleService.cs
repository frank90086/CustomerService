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
            var returnList = Model.AvailableDateStock.Where(x => x.Quantity > 0).Select(x => new AllowTimeTable { Time = x.AvailableDate, Quantity = x.Quantity }).OrderBy(x => x.Time).ToList();
            return returnList;
        }

        public List<AllowTimeTable> GetNotAllowed()
        {
            List<AllowTimeTable> returnList = new List<AllowTimeTable>();
            return returnList;
        }

        public void CreateBooking(List<DateTimeOffset> selections, out bool isFinish)
        {
            MemberBalance balance = Model.Balances.FirstOrDefault();
            List<MemberBooking> bookingList = new List<MemberBooking>();
            long classes = (long) Model.Bookings.Where(x => x.MemberId == balance.MemberId).ToList().Count;
            if (selections.Count > 0 && selections.Count <= balance.AvailableBalances)
            {
                foreach (DateTimeOffset item in selections)
                {
                    var exist = Model.Bookings.Where(x => x.StartDate == item).FirstOrDefault();
                    if (exist == null)
                    {
                        bookingList.Add(new MemberBooking()
                        {
                            Id = PublicMethod.GetToken(),
                                MemberId = balance.MemberId,
                                StartDate = item,
                                EndDate = item.AddHours(1),
                               Classes = classes == 0 ? 1 : classes++,
                                Status = BookingStatus.Booking
                        });
                    }
                }
                Model.Bookings.AddRange(bookingList);
                balance.AvailableBalances -= bookingList.Count;
                isFinish = true;
            }
            else
                isFinish = false;
        }

        public void CreateSchedule(MemberBooking model)
        {
            Model.Schedule.Events.Add(new ScheduleEvent()
            {
                Id = PublicMethod.GetToken(),
                    ScheduleId = Model.Schedule.Id,
                    Name = "Chinese Class",
                    Type = ScheduleEventType.Learning,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Status = ScheduleEventStatus.Accept
            });
        }

        public void MakePair(MemberBooking model, out bool isSuccess)
        {
            TimeSpan machTime = model.StartDate.TimeOfDay;
            MemberBookingAssigner assigner = model.Assigners.LastOrDefault();
            if (_CheckAssignStatus(assigner))
            {
                isSuccess = true;
                return;
            }
            foreach (MemberWorkTime teacher in Model.Teachers)
            {
                MemberWorkTimeItem item = teacher.Times.Where(x => x.Time == machTime).FirstOrDefault();
                if (item != null)
                {
                    model.Assigners.Add(new MemberBookingAssigner()
                    {
                        Id = PublicMethod.GetToken(),
                            MemberId = item.MemberId,
                            MemberBookingId = model.Id,
                            Role = AssignerRole.Teacher,
                            Status = AssignStatus.Assigned
                    });
                    model.Status = BookingStatus.Confirm;
                    _updateStock(item);
                    break;
                }
            }
            assigner = model.Assigners.LastOrDefault();
            if (_CheckAssignStatus(assigner))
                isSuccess = true;
            else
                isSuccess = false;
        }

        private bool _CheckAssignStatus(MemberBookingAssigner assign)
        {
            if (assign == null)
                return false;
            if (assign.Status == AssignStatus.Assigned)
                return true;
            else
                return false;
        }

        private void _updateStock(MemberWorkTimeItem item){
            AvailableDateStock stock = Model.AvailableDateStock.Where(x => x.AvailableDate.DayOfWeek == item.Weekday && x.AvailableDate.Hour == item.Time.Hours).FirstOrDefault();
            if (stock != null)
            {
                AvailableDateStockMember member = stock.Members.Where(x => x.MemberId == item.MemberId).FirstOrDefault();
                stock.Members.Remove(member);
            }
        }

        public void ResetModel(){
            Model = new InitialModel();
        }
    }
}