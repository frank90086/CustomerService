using System;
using System.Collections.Generic;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class AvailableDateStock
    {
        public AvailableDateStock()
        {
            Members = new HashSet<AvailableDateStockMember>();
        }

        public DateTimeOffset AvailableDate { get; set; }
        public string SourceKey { get; set; }
        public TimeStockType Type { get; set; }
        public int Quantity { get {return Members.Count; } private set { } }

        public virtual ICollection<AvailableDateStockMember> Members { get; set; }
    }
}