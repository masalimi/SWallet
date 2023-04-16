using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;

namespace Nop.Plugin.Payments.Wallet.Domain
{
    public class WalletCustomerHistory : BaseEntity
    {
        public int OrderId { get; set; }
        public int StoreId { get; set; }
        public int WalletCustomerId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string TransferType { get; set; }
    }
}
