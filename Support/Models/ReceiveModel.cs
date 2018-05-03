using Omi.Education.Enums.Service;
namespace Omi.Education.Web.Management.Services.Models
{
    public class ReceiveModel
    {
        public CustomerCommand Command { get; set; }
        public string Message { get; set; }
        public string FromId { get; set; }

    }
}