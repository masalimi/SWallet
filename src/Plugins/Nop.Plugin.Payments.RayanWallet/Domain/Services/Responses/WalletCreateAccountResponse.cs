using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Services.Responses
{
    public class WalletCreateAccountResponse : WalletBaseReponse
    {
        public string referenceAccountId { get; set; }
        public string accountId { get; set; }
        
    }
}
