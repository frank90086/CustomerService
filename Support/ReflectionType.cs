using System;
using Omi.Education.Web.Management.Services.Models;
using System.Reflection;
using Omi.Education.Library.SignalR.Connection.HubModel;

namespace Omi.Education.Web.Management.Services
{
    public class ReflectionType
    {
        private SupportService _service;
        private const string baseTypeName = "Omi.Education.Web.Management.Services.";
        private const string tailTypeName = "Cmd";
        public ReflectionType(ISupportService service)
        {
            _service = service as SupportService;
        }

        public BaseCmd GetCmd(CommandModel model, SendContent receviContent)
        {
            Type type = Type.GetType(combinTypeName(model.CustomerCmd.ToString()));
            BaseCmd baseCmd = (BaseCmd) (Activator.CreateInstance(type, _service, receviContent));
            return baseCmd;
        }

        private string combinTypeName(string value)
        {
            return baseTypeName + value + tailTypeName;
        }

        // public Type GetCmdType(CustomerCommand cmd)
        // {
        //     FieldInfo data = typeof(CustomerCommand).GetField(cmd.ToString());
        //     Attribute attribute = Attribute.GetCustomAttribute(data, typeof(CustomerCmdAttribute));
        //     CustomerCmdAttribute valueAttribute = (CustomerCmdAttribute) attribute;
        //     return valueAttribute.CmdType;
        // }
    }
}