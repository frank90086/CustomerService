using System;
using System.Collections.Generic;
using Omi.Education.Enums;

namespace Omi.Education.Web.Management.Services.Models
{
    public class MemberBalance
    {
        public MemberBalance()
        {
            Ranges = new List<MemberBalanceRange>();
            Transactions = new List<MemberBalanceTransaction>();
        }

        public string Id { get; set; }
        public string MemberId { get; set; }
        public BalanceType Type { get; set; }
        public string CurrencyId { get; set; }
        public decimal TotalBalances { get; set; }
        public decimal AvailableBalances { get; set; }
        public DateTimeOffset? ExpiredDate { get; set; }

        public virtual ICollection<MemberBalanceRange> Ranges { get; set; }
        public virtual ICollection<MemberBalanceTransaction> Transactions { get; set; }

    }
}