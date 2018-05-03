using System;
using System.Collections.Generic;
using Omi.Education.Enums.Service;

namespace Omi.Education.Web.Management.Services.Models
{
    public class SupportRequirementModel
    {
        public SupportRequirementModel()
        {
            SupportProcesses = new List<SupportProcessModel>();
            RequireTime = DateTime.Now;
        }
        public string Id { get; set; }
        public string MemberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime RequireTime { get; set; }
        public RequirementStatus Status { get; set; }
        public QusetionType QType { get; set; }
        public string Handler { get; set; }
        public string SupportRequirementId { get; set; }
        public List<SupportProcessModel> SupportProcesses { get; set; }
    }
}