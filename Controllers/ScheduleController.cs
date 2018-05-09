using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Omi.Education.Enums;
using Omi.Education.Enums.Service;
using Omi.Education.Web.Management.Models.ScheduleModels;
using Omi.Education.Web.Management.Services;
using Omi.Education.Web.Management.Services.Models;

namespace Omi.Education.Web.Management.Controllers
{
    public class ScheduleController : Controller
    {
        private ScheduleService _service;
        public ScheduleController(IScheduleService service)
        {
            _service = service as ScheduleService;
        }
        public IActionResult Index()
        {
            ViewBag.AllowTimeTable = PublicMethod.JsonSerialize<List<AllowTimeTable>>(_service.GetAllowTime());
            ViewBag.Point = _service.Model.Balances.FirstOrDefault().AvailableBalances;
            return View();
        }

        public IActionResult Selection(string Selections)
        {
            List<DateTimeOffset> selections = PublicMethod.JsonDeSerialize<List<DateTimeOffset>>(Selections);
            bool isFinish;
            _service.CreateBooking(selections, out isFinish);
            if (!isFinish)
                return RedirectToAction("Index");
            foreach (MemberBooking item in _service.Model.Bookings.ToList())
            {
                MemberBalance balance = _service.Model.Balances.Where(x => x.MemberId == item.MemberId && x.AvailableBalances > 0).FirstOrDefault();
                bool isSuccess;

                if (balance != null && item.Status == BookingStatus.Booking)
                {
                    _service.MakePair(item, out isSuccess);
                    if (isSuccess)
                    {
                        _service.CreateSchedule(item);
                    }
                    else
                        _service.Model.Bookings.Remove(item);
                }
            }
            return RedirectToAction("Schedule", "Schedule");
        }

        public IActionResult Schedule()
        {
            ViewBag.Schedule = PublicMethod.JsonSerialize<Schedule>(_service.Model.Schedule);
            return View();
        }

        public IActionResult ResetModel(){
            _service.ResetModel();
            return RedirectToAction("Index", "Schedule");
        }
    }
}