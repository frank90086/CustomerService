using System;
using System.Collections.Generic;

namespace Omi.Education.Web.Management.Services.Models
{
    public class SupportSolutionModel
    {
        public SupportSolutionModel()
        {
            Id = PublicMethod.GetToken();
            SupportSolutionOptions = new List<SupportSolutionOptionModel>();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string SupportRequirementId { get; set; }
        public List<SupportSolutionOptionModel> SupportSolutionOptions { get; set; }
    }
}