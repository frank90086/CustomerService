using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Omi.Education.Library.SignalR.Connection;
using Omi.Education.Web.Management.Models.CustomerModels;
using Omi.Education.Web.Management.Services;
using Omi.Education.Enums.Service;

namespace Omi.Education.Web.Management.Controllers
{
    public class CustomerController : Controller
    {
        private SupportService _service;
        public CustomerController(ISupportService service)
        {
            _service = service as SupportService;
        }
        public IActionResult Index()
        {
            ViewBag.ServiceToken = _service.Connector.HubToken;
            ViewBag.BaseUri = _service.Connector.baseUri.Replace("Service","Client");
            return View();
        }

        public IActionResult Service(string requireId)
        {
            ViewBag.ServiceToken = _service.Connector.HubToken;
            ViewBag.BaseUri = _service.Connector.baseUri.Replace("Service","Client");
            ViewBag.ReqireId = requireId;
            return View();
        }

        public IActionResult CustomerList()
        {
            ViewBag.ServiceToken = _service.Connector.HubToken;
            ViewBag.BaseUri = _service.Connector.baseUri.Replace("Service","Client");
            CustomerViewModel model = new CustomerViewModel(_service);
            model.RequirementList = _service.Requirements.Where(x => x.QType == QusetionType.Normal).ToList();
            return View(model);
        }

        public IActionResult ITList()
        {
            ViewBag.ServiceToken = _service.Connector.HubToken;
            ViewBag.BaseUri = _service.Connector.baseUri.Replace("Service","Client");
            CustomerViewModel model = new CustomerViewModel(_service);
            model.RequirementList = _service.Requirements.Where(x => x.QType == QusetionType.Technical).ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult _customerList(){
            CustomerViewModel model = new CustomerViewModel(_service);
            model.RequirementList = _service.Requirements.Where(x => x.QType == QusetionType.Normal).ToList();
            return PartialView("_customerList",model);
        }

        [HttpPost]
        public IActionResult _itList(){
            CustomerViewModel model = new CustomerViewModel(_service);
            model.RequirementList = _service.Requirements.Where(x => x.QType == QusetionType.Technical).ToList();
            return PartialView("_itList",model);
        }

        [HttpPost]
        public JsonResult _calculationChart(){
            int wait = _service.Requirements.Where(x => x.Status == RequirementStatus.Wait).ToList().Count;
            int handling = _service.Requirements.Where(x => x.Status == RequirementStatus.Handling).ToList().Count;
            int complete = _service.Requirements.Where(x => x.Status == RequirementStatus.Complete).ToList().Count;
            int runaway = _service.Requirements.Where(x => x.Status == RequirementStatus.Runaway).ToList().Count;
            return Json(new {waitcount=wait,handlingcount=handling,completecount=complete,runawaycount=runaway});
        }

        // [HttpPost]
        // public JsonResult _calculationAppraise(){
        //     int verypoor = CustomerServiceMiddleware._customerInfo.Where(x => x.Appraise == 1).ToList().Count;
        //     int poor = CustomerServiceMiddleware._customerInfo.Where(x => x.Appraise == 2).ToList().Count;
        //     int ok = CustomerServiceMiddleware._customerInfo.Where(x => x.Appraise == 3).ToList().Count;
        //     int good = CustomerServiceMiddleware._customerInfo.Where(x => x.Appraise == 4).ToList().Count;
        //     int verygood = CustomerServiceMiddleware._customerInfo.Where(x => x.Appraise == 5).ToList().Count;
        //     return Json(new {verypoorcount=verypoor,poorcount=poor,okcount=ok,goodcount=good,verygoodcount=verygood});
        // }
    }
}