using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.Wallet.Models.ApiModels
{
    public class WalletBalanceResponse : WalletBaseReponse
    {
        public int Balance { get; set; }
    }
}
