using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Services.Responses
{
    public class ReverseResponse : WalletBaseReponse
    {
        public string referenceNo { get; set; }
        public string captureDate { get; set; }
        public string transactionId { get; set; }
        public DateTime transactionDateTime { get; set; }
        public DateTime reverseDateTime { get; set; }
        public DateTime LocalDateTime { get; set; }
        public string StatusCode { get; set; }
    }
}
