using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Services.Requests
{
    public class RefundRequest
    {
        public string refundAdditionalData { get; set; }
        public string originalReferenceNo { get; set; }
        public DateTime originalLocalDateTime { get; set; }
        public string refundReferenceNo { get; set; }
        public string refundCategory { get; set; }
        public DateTime refundLocalDateTime { get; set; }
    }
}
