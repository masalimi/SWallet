using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Services.Responses
{
    public class WalletBalanceResponse : WalletBaseReponse
    {
        public int Balance { get; set; }
    }
}
