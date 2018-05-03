using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Omi.Education.Library.SignalR.Connection;
using Omi.Education.Library.SignalR.Connection.HubModel;
using Omi.Education.Web.Management.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Omi.Education.Enums.Service;

namespace Omi.Education.Web.Management.Services
{
    public class SupportService : ISupportService
    {
        public OmiHubConnector Connector { get; set; }
        public List<ClientInfoModel> Clients { get; set; }
        public List<SupportRequirementModel> Requirements { get; set; }
        public List<SupportSolutionModel> Solutions { get; set; }
        public List<string> qTypeList { get; set; }
        public SupportService(string baseUri)
        {
            Connector = new OmiHubConnector(baseUri);
            Connector.Register().GetAwaiter().GetResult();
            Connector.CommunicationError += CommunicationErrorHandler;
            Connector.ReceiveMessage += ReceiveMessageHandler;
            Clients = new List<ClientInfoModel>();
            Requirements = new List<SupportRequirementModel>();
            Solutions = new List<SupportSolutionModel>();
            initialSolutionMethod();
            qTypeList = Enum.GetNames(typeof(QusetionType)).ToList();
            Task.Run(() =>
            {
                Echo();
            });
        }

        private void CommunicationErrorHandler(object sender, ErrorEventArgs e)
        {
            if (e.ReTry)
            {
                Connector.Register().GetAwaiter().GetResult();
            }
        }

        private void ReceiveMessageHandler(object sender, ReceiveEventArgs e)
        {
            if (e.ReceiveContent != null)
            {
                List<string> strList = new List<string>();
                CommandModel receiveCmd = PublicMethod.JsonDeSerialize<CommandModel>(e.ReceiveContent.Message);
                ReflectionType typeObject = new ReflectionType(this);
                BaseCmd baseCmd = typeObject.GetCmd(receiveCmd, e.ReceiveContent);
                strList = baseCmd.Do();
                foreach (string strContent in strList)
                {
                    Connector.Send(strContent);
                }
                baseCmd.Dispose();
            }
        }

        public List<string> SendContentMethod(List<string> strList, string targetId, CustomerCommand command, string message = null, string fromId = null)
        {
            SendContent CtoC = new SendContent()
            {
            Target = targetId,
            Message = PublicMethod.JsonSerialize<ReceiveModel>(new ReceiveModel()
            {
            Command = command,
            Message = message,
            FromId = fromId
            }),
            From = Connector.HubToken
            };
            strList.Add(PublicMethod.JsonSerialize<SendContent>(CtoC));
            return strList;
        }

        private void Echo()
        {
            if (Clients.Count > 0)
            {
                List<string> strList = new List<string>();

                foreach (ClientInfoModel client in Clients.ToList())
                {
                    int echoMinutes = DateTime.Now.Subtract(client.EchoTime).Minutes;
                    if (echoMinutes >= 1 && echoMinutes <= 3)
                        strList = SendContentMethod(strList, client.Id, CustomerCommand.Echo);
                    else if (echoMinutes > 3)
                        Clients.Remove(client);
                }
                foreach (string strContent in strList)
                    Connector.Send(strContent);
            }
            Thread.Sleep(5000);
            Echo();
        }

        private void removeClient(List<string> list)
        {
            foreach (string id in list)
            {
                ClientInfoModel client = Clients.Where(x => x.Id == id).FirstOrDefault();
                if (client != null)
                    Clients.Remove(client);
            }
        }

        private void initialSolutionMethod()
        {
            SupportSolutionModel m1 = new SupportSolutionModel()
            {
                Title = "方案A",
                SupportSolutionOptions = new List<SupportSolutionOptionModel>()
                {
                new SupportSolutionOptionModel()
                {
                Description = "退款80%，贈送免費課程十堂"
                },
                new SupportSolutionOptionModel()
                {
                Description = "退款60%，贈送免費課程5堂"
                },
                new SupportSolutionOptionModel()
                {
                Description = "退款40%，贈送免費課程1堂"
                },
                }
            };

            SupportSolutionModel m2 = new SupportSolutionModel()
            {
                Title = "方案B",
                SupportSolutionOptions = new List<SupportSolutionOptionModel>()
                {
                new SupportSolutionOptionModel()
                {
                Description = "申請發票"
                },
                new SupportSolutionOptionModel()
                {
                Description = "申請訂單明細"
                },
                new SupportSolutionOptionModel()
                {
                Description = "申請其他服務"
                },
                }
            };

            SupportSolutionModel m3 = new SupportSolutionModel()
            {
                Title = "是不是想要[林厚吉]的電話",
                SupportSolutionOptions = new List<SupportSolutionOptionModel>()
                {
                new SupportSolutionOptionModel()
                {
                Description = "是"
                },
                new SupportSolutionOptionModel()
                {
                Description = "否"
                }
                }
            };

            SupportSolutionModel m4 = new SupportSolutionModel()
            {
                Title = "請提供您對於此次服務的評價，作為未來改善的依據",
                SupportSolutionOptions = new List<SupportSolutionOptionModel>()
                {
                new SupportSolutionOptionModel()
                {
                Description = "Hen厲害"
                },
                new SupportSolutionOptionModel()
                {
                Description = "Hen好"
                },
                new SupportSolutionOptionModel()
                {
                Description = "一般"
                },
                new SupportSolutionOptionModel()
                {
                Description = "Hen不好"
                },
                new SupportSolutionOptionModel()
                {
                Description = "糟透了"
                }
                }
            };

            Solutions.AddRange(new List<SupportSolutionModel>() { m1, m2, m3, m4 });
        }
    }
}