using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Services.Requests
{
    public class WalletEditBalancesRequest
    {
        public string referenceAccountId { get; set; }
        public int maxDebtorBalance { get; set; }
        public int maxCreditorBalance { get; set; }
    }
}
