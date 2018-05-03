using System;
using System.Collections.Generic;
using Omi.Education.Enums.Service;

namespace Omi.Education.Web.Management.Services.Models
{
    public class SupportProcessModel
    {
        public SupportProcessModel()
        {
            Id = PublicMethod.GetToken();
            SendTime = DateTime.Now;
        }
        public string Id { get; set; }
        public DateTime SendTime { get; set; }
        public SupportProcessStatus Status { get; set; }
        public SupportSolutionModel SupportSolutions { get; set; }
        public string Message { get; set; }
        public string SupportRequirementId { get; set; }
    }
}