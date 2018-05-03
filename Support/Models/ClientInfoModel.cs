using System;
using Omi.Education.Enums.Service;

namespace Omi.Education.Web.Management.Services.Models
{
    public class ClientInfoModel
    {
        public string Id { get; set; }
        public DateTime ConnectTime { get; set; }
        public DateTime EchoTime { get; set; }
        public ClientType ClientType { get; set; }
        public string GroupName { get; set; }
    }
}