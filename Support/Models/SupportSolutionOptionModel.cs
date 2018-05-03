using System;

namespace Omi.Education.Web.Management.Services.Models
{
    public class SupportSolutionOptionModel
    {
        public SupportSolutionOptionModel()
        {
            Id = PublicMethod.GetToken();
            selected = false;
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool selected { get; set; }
        public string SupportSolutionId { get; set; }
    }
}