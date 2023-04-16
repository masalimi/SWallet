using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Data
{
    public class WalletCustomerAmount : BaseEntity
    {
        public int WalletCustomerId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public decimal Amount { get; set; }
        public bool IsApplied { get; set; }
    }
}
