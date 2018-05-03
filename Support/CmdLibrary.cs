using System;
using Omi.Education.Library.SignalR.Connection.HubModel;
using Omi.Education.Web.Management.Services.Models;
using System.Collections.Generic;
using System.Linq;
using Omi.Education.Enums.Service;

namespace Omi.Education.Web.Management.Services
{
    public abstract class BaseCmd : IDisposable
    {
        internal SupportService _service;
        internal SendContent _receiveContent;
        internal CommandModel _receiveCmd;
        internal List<string> strList = new List<string>();
        public BaseCmd(ISupportService service, SendContent receiveContent)
        {
            _service = service as SupportService;
            _receiveContent = receiveContent;
            _receiveCmd = PublicMethod.JsonDeSerialize<CommandModel>(_receiveContent.Message);
        }
        public virtual List<string> Do()
        {
            return strList;
        }

        internal List<string> ReNewList(List<string> strList)
        {
            var List = _service.Clients.Where(x => x.ClientType == ClientType.List).ToList();
            if (List.Count > 0)
            {
                foreach (var list in List)
                {
                    SendContent CtoL = new SendContent()
                    {
                        Target = list.Id,
                        Message = PublicMethod.JsonSerialize<ReceiveModel>(new ReceiveModel()
                        {
                        Command = CustomerCommand.Reload
                        }),
                        From = _service.Connector.HubToken
                    };
                    strList.Add(PublicMethod.JsonSerialize<SendContent>(CtoL));
                }
            }
            return strList;
        }

        internal List<string> historyContentMethod(SupportRequirementModel model, List<string> strList, SupportProcessStatus status, string target)
        {
            if (!String.IsNullOrEmpty(model.SupportRequirementId))
            {
                SupportRequirementModel parentModel = _service.Requirements.Where(x => x.Id == model.SupportRequirementId).FirstOrDefault();
                strList = historyContentMethod(parentModel, strList, status, target);
            }

            foreach (var item in model.SupportProcesses)
            {
                SupportHistoryModel m = new SupportHistoryModel()
                {
                    Status = item.Status,
                };
                switch (item.Status)
                {
                    case SupportProcessStatus.Solution:
                        m.Message = PublicMethod.JsonSerialize<SupportSolutionOptionModel>(item.SupportSolutions.SupportSolutionOptions.Where(x => x.selected == true).FirstOrDefault());
                        break;
                    case SupportProcessStatus.CustomerReply:
                        m.Name = model.MemberId;
                        m.Message = item.Message;
                        break;
                    case SupportProcessStatus.HandlerReply:
                        m.Name = model.Handler;
                        m.Message = item.Message;
                        break;
                }
                string historyStr = PublicMethod.JsonSerialize<SupportHistoryModel>(m);
                strList = _service.SendContentMethod(strList, target, CustomerCommand.History, historyStr);
            }
            return strList;
        }

        internal void AddSolution(string requirId, string solutionId, out SupportRequirementModel requir)
        {
            SupportSolutionModel solution = _service.Solutions.Where(x => x.Id == solutionId).FirstOrDefault();
            requir = _service.Requirements.Where(x => x.Id == requirId).FirstOrDefault();
            SupportProcessModel model = new SupportProcessModel()
            {
                Status = SupportProcessStatus.Solution,
                SupportSolutions = new SupportSolutionModel()
                {
                Id = solution.Id,
                Title = solution.Title,
                SupportSolutionOptions = new List<SupportSolutionOptionModel>()
                }
            };
            foreach (SupportSolutionOptionModel item in solution.SupportSolutionOptions)
            {
                model.SupportSolutions.SupportSolutionOptions.Add(new SupportSolutionOptionModel()
                {
                    Id = item.Id,
                        Description = item.Description
                });
            }
            requir.SupportProcesses.Add(model);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    public class EchoCmd : BaseCmd
    {
        public EchoCmd(ISupportService service, SendContent receivContent) : base(service, receivContent) { }

        public override List<string> Do()
        {

            ClientInfoModel echoClient = _service.Clients.Where(x => x.Id == _receiveContent.From).FirstOrDefault();
            if (echoClient != null)
                echoClient.EchoTime = DateTime.Now;
            return strList;
        }
    }

    public class RegisterCmd : BaseCmd
    {
        public RegisterCmd(ISupportService service, SendContent receivContent) : base(service, receivContent) { }

        public override List<string> Do()
        {
            ClientInfoModel client = PublicMethod.JsonDeSerialize<ClientInfoModel>(_receiveCmd.Content);
            client.ConnectTime = DateTime.Now;
            client.EchoTime = DateTime.Now;
            _service.Clients.Add(client);
            strList = _service.SendContentMethod(strList, client.Id, CustomerCommand.Register);
            return strList;
        }
    }

    public class RequirementCmd : BaseCmd
    {
        public RequirementCmd(ISupportService service, SendContent receivContent) : base(service, receivContent) { }

        public override List<string> Do()
        {
            SupportRequirementModel requirement = PublicMethod.JsonDeSerialize<SupportRequirementModel>(_receiveCmd.Content);
            if (!String.IsNullOrEmpty(requirement.Handler))
            {
                var alreadyRequirement = _service.Requirements.Where(x => x.Id == requirement.Id).FirstOrDefault();
                alreadyRequirement.Handler = requirement.Handler;
                alreadyRequirement.Status = RequirementStatus.Handling;
                strList = historyContentMethod(alreadyRequirement, strList, SupportProcessStatus.HandlerReply, requirement.Handler);
                string groupName = PublicMethod.GetToken();
                _service.Connector.Subscribe(groupName);
                strList = _service.SendContentMethod(strList, alreadyRequirement.MemberId, CustomerCommand.Pair, groupName);
                strList = _service.SendContentMethod(strList, alreadyRequirement.Handler, CustomerCommand.Pair, groupName);
            }
            else
            {
                var oldRequirement = _service.Requirements.Where(x => x.Id == requirement.Id).FirstOrDefault();

                if (oldRequirement != null)
                {
                    oldRequirement.MemberId = requirement.MemberId;
                    oldRequirement.Status = RequirementStatus.Wait;
                    strList = historyContentMethod(oldRequirement, strList, SupportProcessStatus.CustomerReply, requirement.MemberId);
                }
                else
                {
                    requirement.Id = PublicMethod.GetToken();
                    requirement.Status = RequirementStatus.Wait;
                    _service.Requirements.Add(requirement);
                }
                strList = _service.SendContentMethod(strList, requirement.MemberId, CustomerCommand.Requirement, requirement.Id);
            }
            strList = ReNewList(strList);
            return strList;
        }
    }

    public class PairCmd : BaseCmd
    {
        public PairCmd(ISupportService service, SendContent receivContent) : base(service, receivContent) { }
    }

    public class ReplyCmd : BaseCmd
    {
        public ReplyCmd(ISupportService service, SendContent receivContent) : base(service, receivContent) { }

        public override List<string> Do()
        {
            SupportProcessModel process = PublicMethod.JsonDeSerialize<SupportProcessModel>(_receiveCmd.Content);
            _service.Requirements.Where(x => x.Id == process.SupportRequirementId).FirstOrDefault().SupportProcesses.Add(process);
            strList = _service.SendContentMethod(strList, _receiveCmd.Group, CustomerCommand.Reply, process.Message, _receiveContent.From);
            return strList;
        }
    }

    public class WaitCmd : BaseCmd
    {
        public WaitCmd(ISupportService service, SendContent receivContent) : base(service, receivContent) { }

        public override List<string> Do()
        {
            return base.Do();
        }
    }

    public class ReloadCmd : BaseCmd
    {
        public ReloadCmd(ISupportService service, SendContent receivContent) : base(service, receivContent) { }

        public override List<string> Do()
        {
            return base.Do();
        }
    }

    public class GetQusetionTypeCmd : BaseCmd
    {
        public GetQusetionTypeCmd(ISupportService service, SendContent receivContent) : base(service, receivContent) { }

        public override List<string> Do()
        {
            string qTypeStr = String.Join(',', _service.qTypeList);
            strList = _service.SendContentMethod(strList, _receiveCmd.Content, CustomerCommand.GetQusetionType, qTypeStr);
            return strList;
        }
    }

    public class TransHandleCmd : BaseCmd
    {
        public TransHandleCmd(ISupportService service, SendContent receivContent) : base(service, receivContent) { }

        public override List<string> Do()
        {
            TransHandleModel transModel = PublicMethod.JsonDeSerialize<TransHandleModel>(_receiveCmd.Content);
            SupportRequirementModel preTransRequirement = _service.Requirements.Where(x => x.Id == transModel.RequirementId).FirstOrDefault();
            SupportRequirementModel transRequirement = new SupportRequirementModel()
            {
                Id = PublicMethod.GetToken(),
                SupportRequirementId = preTransRequirement.Id,
                MemberId = preTransRequirement.MemberId,
                Name = preTransRequirement.Name,
                Email = preTransRequirement.Email,
                Status = RequirementStatus.Wait,
                QType = transModel.QType
            };
            _service.Requirements.Add(transRequirement);
            preTransRequirement.Status = RequirementStatus.TransHandle;
            strList = _service.SendContentMethod(strList, preTransRequirement.MemberId, CustomerCommand.Requirement, transRequirement.Id);
            strList = _service.SendContentMethod(strList, preTransRequirement.Handler, CustomerCommand.Close, preTransRequirement.QType.ToString());
            strList = ReNewList(strList);
            return strList;
        }
    }

    public class HistoryCmd : BaseCmd
    {
        public HistoryCmd(ISupportService service, SendContent receivContent) : base(service, receivContent) { }

        public override List<string> Do()
        {
            var historyRequirement = _service.Requirements.Where(x => x.Id == _receiveCmd.Content).FirstOrDefault();
            strList = historyContentMethod(historyRequirement, strList, SupportProcessStatus.HandlerReply, _receiveContent.From);
            return strList;
        }
    }

    public class GetSolutionCmd : BaseCmd
    {
        public GetSolutionCmd(ISupportService service, SendContent receivContent) : base(service, receivContent) { }

        public override List<string> Do()
        {
            List<SupportSolutionModel> list = new List<SupportSolutionModel>();
            foreach (SupportSolutionModel item in _service.Solutions)
                list.Add(item);
            strList = _service.SendContentMethod(strList, _receiveCmd.Content, CustomerCommand.GetSolution, PublicMethod.JsonSerialize<List<SupportSolutionModel>>(list));
            return strList;
        }
    }

    public class CheckSolutionCmd : BaseCmd
    {
        public CheckSolutionCmd(ISupportService service, SendContent receivContent) : base(service, receivContent) { }

        public override List<string> Do()
        {
            string[] strArrayC = _receiveCmd.Content.Split(';');
            SupportRequirementModel requirC;
            AddSolution(strArrayC[0], strArrayC[1], out requirC);
            SupportSolutionModel solutionC = requirC.SupportProcesses.Where(x => x.Status == SupportProcessStatus.Solution).LastOrDefault().SupportSolutions;
            strList = _service.SendContentMethod(strList, requirC.MemberId, CustomerCommand.GetSolution, PublicMethod.JsonSerialize<SupportSolutionModel>(solutionC));
            return strList;
        }
    }

    public class SelectSolutionCmd : BaseCmd
    {
        public SelectSolutionCmd(ISupportService service, SendContent receivContent) : base(service, receivContent) { }

        public override List<string> Do()
        {
            string[] strArrayS = _receiveCmd.Content.Split(';');
            SupportRequirementModel requirS = _service.Requirements.Where(x => x.Id == strArrayS[0]).FirstOrDefault();
            SupportSolutionModel solutionS = requirS.SupportProcesses.Where(x => x.Status == SupportProcessStatus.Solution).LastOrDefault().SupportSolutions;
            var optionS = solutionS.SupportSolutionOptions.Where(x => x.Id == strArrayS[1]).FirstOrDefault();
            optionS.selected = true;
            optionS.Title = solutionS.Title;
            strList = _service.SendContentMethod(strList, requirS.MemberId, CustomerCommand.SelectSolution, PublicMethod.JsonSerialize<SupportSolutionOptionModel>(optionS));
            strList = _service.SendContentMethod(strList, requirS.Handler, CustomerCommand.SelectSolution, PublicMethod.JsonSerialize<SupportSolutionOptionModel>(optionS));
            return strList;
        }
    }

    public class CloseCmd : BaseCmd
    {
        public CloseCmd(ISupportService service, SendContent receivContent) : base(service, receivContent) { }

        public override List<string> Do()
        {
            SupportRequirementModel requirE = _service.Requirements.Where(x => x.Id == _receiveCmd.Content).FirstOrDefault();
            requirE.Status = RequirementStatus.Complete;
            strList = _service.SendContentMethod(strList, requirE.MemberId, CustomerCommand.Close, "");
            strList = _service.SendContentMethod(strList, requirE.Handler, CustomerCommand.Close, requirE.QType.ToString());
            strList = ReNewList(strList);
            return strList;
        }
    }
}