using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Data
{
    public class WalletCustomerHistory : BaseEntity
    {
        public int OrderId { get; set; }
        public int? StoreId { get; set; }
        public int WalletCustomerId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string TransactionType { get; set; }
        public decimal? Amount { get; set; }
        public string RefNo { get; set; }
    }
}
