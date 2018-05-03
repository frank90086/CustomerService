using System;
using System.Collections.Generic;
using System.Linq;
using Omi.Education.Web.Management.Services;
using Omi.Education.Web.Management.Services.Models;
using Omi.Education.Enums.Service;

namespace Omi.Education.Web.Management.Models.CustomerModels
{
    public class CustomerViewModel
    {
        public List<SupportRequirementModel> RequirementList { get; set; }
        private SupportService _service;
        public CustomerViewModel(ISupportService service)
        {
            _service = service as SupportService;
        }
        public int Wait
        {
            get
            {
               return _service.Requirements.Count > 0 ? _service.Requirements.Where(x => x.Status == RequirementStatus.Wait).ToList().Count : 0;
            }
            private set { }
        }
        public int Handling
        {
            get
            {
               return _service.Requirements.Count > 0 ? _service.Requirements.Where(x => x.Status == RequirementStatus.Handling).ToList().Count : 0;
            }
            private set { }
        }
        public int Complete
        {
            get
            {
               return _service.Requirements.Count > 0 ? _service.Requirements.Where(x => x.Status == RequirementStatus.Complete).ToList().Count : 0;
            }
            private set { }
        }
        public int Runaway
        {
            get
            {
               return _service.Requirements.Count > 0 ? _service.Requirements.Where(x => x.Status == RequirementStatus.Runaway).ToList().Count : 0;
            }
            private set { }
        }
        // public int VeryPoor
        // {
        //     get
        //     {
        //        return CustomerServiceMiddleware._customerInfo.Count > 0 ? CustomerServiceMiddleware._customerInfo.Where(x => x.Appraise == 1).ToList().Count : 0;
        //     }
        //     private set { }
        // }
        // public int Poor
        // {
        //     get
        //     {
        //        return CustomerServiceMiddleware._customerInfo.Count > 0 ? CustomerServiceMiddleware._customerInfo.Where(x => x.Appraise == 2).ToList().Count : 0;
        //     }
        //     private set { }
        // }
        // public int Ok
        // {
        //     get
        //     {
        //        return CustomerServiceMiddleware._customerInfo.Count > 0 ? CustomerServiceMiddleware._customerInfo.Where(x => x.Appraise == 3).ToList().Count : 0;
        //     }
        //     private set { }
        // }
        // public int Good
        // {
        //     get
        //     {
        //        return CustomerServiceMiddleware._customerInfo.Count > 0 ? CustomerServiceMiddleware._customerInfo.Where(x => x.Appraise == 4).ToList().Count : 0;
        //     }
        //     private set { }
        // }
        // public int VeryGood
        // {
        //     get
        //     {
        //        return CustomerServiceMiddleware._customerInfo.Count > 0 ? CustomerServiceMiddleware._customerInfo.Where(x => x.Appraise == 5).ToList().Count : 0;
        //     }
        //     private set { }
        // }
    }
}