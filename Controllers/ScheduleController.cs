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
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;
using TimeZoneConverter;

namespace Omi.Education.Web.Management.Controllers
{
    public class ScheduleController : Controller
    {
        private ScheduleService _service;
        private IHttpContextAccessor _accessor;
        private CultureInfo _culture;
        private TimeZoneInfo _customerTimezone;
        private TimeZoneInfo _systemTimezone;
        public ScheduleController(IScheduleService service, IHttpContextAccessor accessor)
        {
            _service = service as ScheduleService;
            _accessor = accessor;

            var zoneId = _accessor.HttpContext.Request.Cookies["tzTime"] ?? "Asia/Taipei";
            string tz = TZConvert.IanaToWindows(zoneId);
            _customerTimezone = TimeZoneInfo.FindSystemTimeZoneById(tz);
            _systemTimezone = TimeZoneInfo.Local;
            var cultureId = _accessor.HttpContext.Request.Cookies["culture"] ?? "zh-TW";
            var cultureInfo = CultureInfo.CreateSpecificCulture(cultureId);
            _culture = cultureInfo;
        }
        public IActionResult Index()
        {
            List<AllowTimeTable> list = _service.GetAllowTime();
            foreach (var item in list)
            {
                item.Time = TimeZoneInfo.ConvertTime(item.Time, _customerTimezone);
            }
            ViewBag.AllowTimeTable = PublicMethod.JsonSerialize<List<AllowTimeTable>>(list);
            ViewBag.Point = _service.Model.Balances.FirstOrDefault().AvailableBalances;
            return View();
        }

        public IActionResult Selection(string Selections)
        {
            List<DateTimeOffset> selections = PublicMethod.JsonDeSerialize<List<DateTimeOffset>>(Selections);
            List<DateTimeOffset> convertSelections = new List<DateTimeOffset>();
            foreach (var item in selections)
            {
                convertSelections.Add(TimeZoneInfo.ConvertTime(item, _systemTimezone));
            }
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
            foreach (var item in _service.Model.Schedule.Events)
            {
                item.StartDate = TimeZoneInfo.ConvertTime(item.StartDate, _customerTimezone);
                item.EndDate = TimeZoneInfo.ConvertTime(item.EndDate, _customerTimezone);
            }
            ViewBag.Schedule = PublicMethod.JsonSerialize<Schedule>(_service.Model.Schedule);
            return View();
        }

        public IActionResult ResetModel()
        {
            _service.ResetModel();
            return RedirectToAction("Index", "Schedule");
        }


        public JsonResult SetTimeZoneCultureCookie(string time, string culture)
        {
            _accessor.HttpContext.Response.Cookies.Append("tzTime", time);
            _accessor.HttpContext.Response.Cookies.Append("culture", culture);
            return Json(new { StandardName = TimeZoneInfo.Local.StandardName, id = TimeZoneInfo.Local.Id, displayname = TimeZoneInfo.Local.DisplayName});
        }
    }
}