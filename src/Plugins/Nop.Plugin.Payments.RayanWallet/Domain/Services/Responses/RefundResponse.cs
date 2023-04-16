using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Services.Responses
{
    public class RefundResponse : WalletBaseReponse
    {
        public string refundReferenceNo { get; set; }
        public DateTime refundCaptureDate { get; set; }
        public string refundTransactionId { get; set; }
        public DateTime refundTransactionDateTime { get; set; }
        public DateTime refundLocalDateTime { get; set; }
    }
}
