using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Data

{
    public class WalletCustomer : BaseEntity
    {
        //public int Amount { get; set; }
        public string Username { get; set; }
        public int StoreId { get; set; }
        public string ReferenceAccountId { get; set; }
        public int SourceId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Active { get; set; }
        //public bool IsChange { get; set; }
    }
}
