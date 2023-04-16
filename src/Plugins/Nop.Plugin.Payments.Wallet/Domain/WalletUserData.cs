using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;

namespace Nop.Plugin.Payments.Wallet.Domain
{
    public partial class WalletUserData : BaseEntity
    {
        public string CardNo { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string Cvv2 { get; set; }
        public string UserName { get; set; }
        public long Amount { get; set; }
        //public int CustomerId { get; set; }
        //public int StoreId { get; set; }
        //public int? ReferenceAccountId { get; set; }
        //public int SourceId { get; set; }
        //public DateTime CreateDate { get; set; }
        //public DateTime UpdateDate { get; set; }
        //public bool Active { get; set; }

    }
}
