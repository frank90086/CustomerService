using System.Collections.Generic;
using Omi.Education.Library.SignalR.Connection;
using Omi.Education.Web.Management.Services.Models;

namespace Omi.Education.Web.Management.Services
{
    public interface ISupportService
    {
        OmiHubConnector Connector { get; set;}
        List<ClientInfoModel> Clients { get; set; }
    }
}