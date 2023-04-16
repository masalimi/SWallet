using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Services.Requests
{
    public class WalletBalanceRequest
    {
        public string referenceAccountId { get; set; }
    }
}
