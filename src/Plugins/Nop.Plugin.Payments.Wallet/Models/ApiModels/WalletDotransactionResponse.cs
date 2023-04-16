using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.Wallet.Models.ApiModels
{
    public class WalletDotransactionResponse : WalletBaseReponse
    {

        public string referenceNo { get; set; }
        public string captureDate { get; set; }
        public string transactionId { get; set; }
        public DateTime transactionDateTime { get; set; }
        public DateTime localDateTime { get; set; }
        public string StatusCode { get; set; }
    }
}
