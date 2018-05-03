using Omi.Education.Enums.Service;
namespace Omi.Education.Web.Management.Services.Models
{
    public class SupportHistoryModel
    {
        public SupportProcessStatus Status { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
    }
}