using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Omi.Education.Web.Management.Services;
using Omi.Education.Enums.Service;
using Omi.Education.Web.Management.Services.Models;
using Omi.Education.Web.Management.Models.ScheduleModels;

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
            List<DateTime> selections = PublicMethod.JsonDeSerialize<List<DateTime>>(Selections);
            bool _isPair;
            _service.MakePair(selections, out _isPair);
            if (!_isPair)
               return RedirectToAction("Index");
            else
               return View();
        }
    }
}