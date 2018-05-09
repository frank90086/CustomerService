using System;
using System.Collections.Generic;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class AllowTimeTable
    {
        public int Quantity { get; set; }
        public DateTimeOffset Time { get; set; }
    }
}