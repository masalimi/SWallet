using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;

namespace Nop.Plugin.Payments.Wallet.Domain
{
    public class WalletCustomer : BaseEntity
    {
        public int Amount { get; set; }
        public int CustomerId { get; set; }
        public int StoreId { get; set; }
        public string ReferenceAccountId { get; set; }
        public int SourceId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Active { get; set; }
    }
}
