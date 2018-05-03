using System;
using Omi.Education.Enums.Service;

namespace Omi.Education.Web.Management.Services.Models
{
    public class CommandModel
    {
        public CustomerCommand CustomerCmd { get; set; }
        public string Content { get; set; }
        public string Group { get; set; }
    }
}