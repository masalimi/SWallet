using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.Wallet.Models.ApiModels
{
    public class WalletBaseReponse
    {

        public bool Succeeded { get; set; }
        public string ResponseCode { get; set; }
        public List<string> error { get; set; }
        public string errorCode { get; set; }
        public string errorDescription { get; set; }

    }
}
