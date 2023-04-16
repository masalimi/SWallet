using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.Wallet.Models.ApiModels
{
    public class WalletCreateAccountResponse : WalletBaseReponse
    {
        public string referenceAccountId { get; set; }
        public string accountId { get; set; }
        public string StatusCode { get; set; }
    }
}
