using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public List<AllowTimeTableViewModel> GetAllowTime(TimeZoneInfo timezone)
        {
            var returnList = Model.AvailableDateStock.Where(x => x.Quantity > 0).Select(x => new AllowTimeTableViewModel(timezone) { Time = x.AvailableDate, Quantity = x.Quantity }).OrderBy(x => x.Time).ToList();
            return returnList;
        }

        public List<AllowTimeTableViewModel> GetNotAllowed()
        {
            List<AllowTimeTableViewModel> returnList = new List<AllowTimeTableViewModel>();
            return returnList;
        }

        public List<ScheduleViewModel> GetSchedule(TimeZoneInfo timezone)
        {
            Schedule student = Model.Schedules.FirstOrDefault(x => x.MemberId == Model.StudentId);
            List<ScheduleViewModel> returnList = student.Events.Select(x => new ScheduleViewModel(timezone)
            {
                Name = student.Name,
                    Type = student.Type,
                    Color = student.Color,
                    StartDate = x.StartDate
            }).ToList();
            return returnList;
        }

        public List<SelectListItem> GetTeacherList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (MemberWorkTime item in Model.Teachers)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.MemberId,
                        Value = item.MemberId
                });
            }
            return list;
        }

        public void CreateBooking(List<SelectionsViewModel> selections, out bool isFinish)
        {
            MemberBalance balance = Model.Balances.FirstOrDefault();
            List<MemberBooking> bookingList = new List<MemberBooking>();
            long classes = (long) Model.Bookings.Where(x => x.MemberId == balance.MemberId).ToList().Count;
            if (selections.Count > 0 && selections.Count <= balance.AvailableBalances)
            {
                foreach (SelectionsViewModel item in selections)
                {
                    var exist = Model.Bookings.FirstOrDefault(x => x.StartDate == item.SelectTime);
                    if (exist == null)
                    {
                        bookingList.Add(new MemberBooking()
                        {
                            Id = PublicMethod.GetToken(),
                                MemberId = balance.MemberId,
                                StartDate = item.SelectTime,
                                EndDate = item.SelectTime.AddHours(1),
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
            Schedule student = Model.Schedules.FirstOrDefault(x => x.MemberId == Model.StudentId);
            Schedule teacher = Model.Schedules.FirstOrDefault(x => x.MemberId == model.Assigners.LastOrDefault().MemberId);
            student.Events.Add(new ScheduleEvent()
            {
                Id = PublicMethod.GetToken(),
                    ScheduleId = student.Id,
                    Name = "Chinese Class",
                    Type = ScheduleEventType.Learning,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Status = ScheduleEventStatus.Accept,
                    Source = "MemberBooking",
                    SourceKey = model.Id,
                    SourceKey2 = teacher.Id
            });

            teacher.Events.Add(new ScheduleEvent()
            {
                Id = PublicMethod.GetToken(),
                    ScheduleId = teacher.Id,
                    Name = "Chinese Class",
                    Type = ScheduleEventType.Learning,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Status = ScheduleEventStatus.Accept,
                    Source = "MemberBooking",
                    SourceKey = model.Id,
                    SourceKey2 = student.Id
            });
        }

        public void CheckException(string id, SelectionsViewModel selection, out bool hasClass)
        {
            Schedule teacher = Model.Schedules.FirstOrDefault(x => x.MemberId == id);
            var tEvent = teacher.Events.FirstOrDefault(x => x.StartDate == selection.SelectTime);
            if (tEvent != null)
                hasClass = true;
            else
                hasClass = false;
        }

        public void CreateException(string id, SelectionsViewModel selection, out bool isFinish)
        {
            var workTime = Model.Teachers.FirstOrDefault(x => x.MemberId == id);
            if (workTime != null)
            {
                var workTimeItem = workTime.Times.FirstOrDefault(x => x.Weekday == selection.SelectTime.DayOfWeek && x.Time == selection.SelectTime.TimeOfDay);
                workTimeItem.Exceptions.Add(new MemberWorkTimeItemException()
                {
                    MemberWorkTimeItemId = workTimeItem.Id,
                        ExceptionDate = selection.SelectTime,
                        Status = Status.Disabled
                });
                _removeStock(workTimeItem);
                MemberBooking booking;
                _cancelSchedule(id, selection, out booking);
                bool isSuccess;
                MakePair(booking, out isSuccess);
                if (isSuccess)
                {
                    CreateSchedule(booking);
                    isFinish = true;
                }
                else
                    isFinish = false;
            }
            else
                isFinish = false;
        }

        public void CancelException(string id, SelectionsViewModel selection, out bool isFinish)
        {
            var workTime = Model.Teachers.Where(x => x.MemberId == id).FirstOrDefault();
            if (workTime != null)
            {
                var workTimeItem = workTime.Times.FirstOrDefault(x => x.Weekday == selection.SelectTime.DayOfWeek && x.Time == selection.SelectTime.TimeOfDay);
                workTimeItem.Exceptions.Add(new MemberWorkTimeItemException()
                {
                    MemberWorkTimeItemId = workTimeItem.Id,
                        ExceptionDate = selection.SelectTime,
                        Status = Status.Enabled
                });
                _addStock(workTimeItem);
                isFinish = true;
            }
            else
                isFinish = false;
        }

        public void MakePair(MemberBooking model, out bool isSuccess)
        {
            if (model != null)
            {
                TimeSpan machTime = model.StartDate.TimeOfDay;
                DayOfWeek machWeek = model.StartDate.DayOfWeek;
                MemberBookingAssigner assigner = model.Assigners.LastOrDefault();
                if (_CheckAssignStatus(assigner))
                {
                    isSuccess = true;
                    return;
                }
                foreach (MemberWorkTime teacher in Model.Teachers)
                {
                    MemberWorkTimeItem item = teacher.Times.FirstOrDefault(x => x.Time == machTime && x.Weekday == machWeek);
                    if (item != null)
                    {
                        if (item.Exceptions.Count > 0)
                        {
                            if (item.Exceptions.LastOrDefault().Status == Status.Disabled)
                                continue;
                        }
                        model.Assigners.Add(new MemberBookingAssigner()
                        {
                            Id = PublicMethod.GetToken(),
                                MemberId = item.MemberId,
                                MemberBookingId = model.Id,
                                Role = AssignerRole.Teacher,
                                Status = AssignStatus.Assigned
                        });
                        model.Status = BookingStatus.Confirm;
                        _removeStock(item);
                        break;
                    }
                }
                assigner = model.Assigners.LastOrDefault();
                if (_CheckAssignStatus(assigner))
                    isSuccess = true;
                else
                    isSuccess = false;
            }
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

        private void _removeStock(MemberWorkTimeItem item)
        {
            AvailableDateStock stock = Model.AvailableDateStock.FirstOrDefault(x => x.AvailableDate.DayOfWeek == item.Weekday && x.AvailableDate.Hour == item.Time.Hours);
            if (stock != null)
            {
                AvailableDateStockMember member = stock.Members.FirstOrDefault(x => x.MemberId == item.MemberId);
                stock.Members.Remove(member);
            }
        }

        private void _addStock(MemberWorkTimeItem item)
        {
            AvailableDateStock stock = Model.AvailableDateStock.FirstOrDefault(x => x.AvailableDate.DayOfWeek == item.Weekday && x.AvailableDate.Hour == item.Time.Hours);
            if (stock != null)
            {
                stock.Members.Add(new AvailableDateStockMember()
                {
                    AvailableDate = stock.AvailableDate,
                        Type = TimeStockType.Course,
                        MemberId = item.MemberId
                });
            }
        }

        private void _cancelSchedule(string id, SelectionsViewModel selection, out MemberBooking booking)
        {
            var tSchedule = Model.Schedules.FirstOrDefault(x => x.MemberId == id);
            if (tSchedule != null)
            {
                var tEventItem = tSchedule?.Events.FirstOrDefault(x => x.StartDate == selection.SelectTime);
                if (tEventItem != null)
                {
                    var sSchedule = Model.Schedules.FirstOrDefault(x => x.Id == tEventItem.SourceKey2);
                    if (sSchedule != null)
                    {
                        var sEventItem = sSchedule?.Events.FirstOrDefault(x => x.StartDate == selection.SelectTime);
                        if (sEventItem != null)
                        {
                            booking = Model.Bookings.FirstOrDefault(x => x.Id == tEventItem.SourceKey);
                            booking.Status = BookingStatus.Booking;
                            booking.Assigners.LastOrDefault().Status = AssignStatus.Cancel;
                            tSchedule.Events.Remove(tEventItem);
                            sSchedule.Events.Remove(sEventItem);
                        }
                        else
                            booking = null;
                    }
                    else
                        booking = null;
                }
                else
                    booking = null;
            }
            else
                booking = null;
        }

        public void ResetModel()
        {
            Model = new InitialModel();
        }
    }
}