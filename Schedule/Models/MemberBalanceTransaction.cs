using System;
using System.Collections.Generic;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class MemberBalanceTransaction
    {
        public string Id { get; set; }
        public string BalanceId { get; set; }
        public TransactionType Type { get; set; }
        public string Source { get; set; }
        public string SourceKey { get; set; }
        public string SourceKey2 { get; set; }
        public string SourceKey3 { get; set; }
        public decimal BeforeTotalBalances { get; set; }
        public decimal BeforeAvailableBalances { get; set; }
        public decimal TotalBalances { get; set; }
        public decimal AvailableBalances { get; set; }
    }
}