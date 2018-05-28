using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Omi.Education.Enums;
using Omi.Education.Enums.Service;
using Omi.Education.Web.Management.Models.ScheduleModels;
using Omi.Education.Web.Management.Services;
using Omi.Education.Web.Management.Services.Models;
using TimeZoneConverter;

namespace Omi.Education.Web.Management.Controllers
{
    public class ScheduleController : Controller
    {
        private IScheduleService _service;
        private IHttpContextAccessor _accessor;
        private CultureInfo _culture;
        private TimeZoneInfo _customerTimezone;
        private TimeZoneInfo _systemTimezone;
        public ScheduleController(IScheduleService service, IHttpContextAccessor accessor)
        {
            _service = service;
            _accessor = accessor;

            var zoneId = _accessor.HttpContext.Request.Cookies["tzTime"] ?? "Asia/Taipei";
            _customerTimezone = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(zoneId));
            _systemTimezone = TimeZoneInfo.Local;

            var cultureId = _accessor.HttpContext.Request.Cookies["culture"] ?? "zh-TW";
            _culture = CultureInfo.CreateSpecificCulture(cultureId);
        }
        public IActionResult Index()
        {
            List<AllowTimeTableViewModel> list = _service.GetAllowTime(_customerTimezone);
            ViewBag.AllowTimeTable = PublicMethod.JsonSerialize<List<AllowTimeTableViewModel>>(list);
            ViewBag.Point = _service.Model.Balances.FirstOrDefault().AvailableBalances;
            return View();
        }

        public IActionResult Selection(string Selections)
        {
            List<SelectionsViewModel> selections = PublicMethod.JsonDeSerialize<List<SelectionsViewModel>>(Selections);
            bool isFinish;
            _service.CreateBooking(selections, out isFinish);
            if (!isFinish)
                return RedirectToAction("Index");
            foreach (MemberBooking item in _service.Model.Bookings.ToList())
            {
                MemberBalance balance = _service.Model.Balances.FirstOrDefault(x => x.MemberId == item.MemberId && x.AvailableBalances > 0);
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
            ViewBag.Schedule = PublicMethod.JsonSerialize<List<ScheduleViewModel>>(_service.GetSchedule(_customerTimezone));
            return View();
        }

        public IActionResult Exception()
        {
            ViewBag.Teachers = _service.GetTeacherList();
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
            return Json(new { });
        }

        [HttpPost]
        public JsonResult GetTeacherWorkTime(string id)
        {
            var workTime = _service.Model.Teachers.Where(x => x.MemberId == id).FirstOrDefault();
            return Json(workTime);
        }

        [HttpPost]
        public JsonResult CheckException(string Id, DateTimeOffset selection)
        {
            SelectionsViewModel selectTime = new SelectionsViewModel() { SelectTime = selection };
            bool hasClass = false;
            _service.CheckException(Id, selectTime, out hasClass);
            return Json(new { info = hasClass });
        }

        [HttpPost]
        public JsonResult ConfirmException(string Id, DateTimeOffset selection)
        {
            SelectionsViewModel selectTime = new SelectionsViewModel() { SelectTime = selection };
            bool isFinish = false;
            _service.CreateException(Id, selectTime, out isFinish);
            return Json(new { info = isFinish });
        }

        [HttpPost]
        public JsonResult CancelException(string id, DateTimeOffset selection)
        {
            SelectionsViewModel selectTime = new SelectionsViewModel() { SelectTime = selection };
            bool isFinish;
            _service.CancelException(id, selectTime, out isFinish);
            return Json(new { info = isFinish });
        }
    }
}