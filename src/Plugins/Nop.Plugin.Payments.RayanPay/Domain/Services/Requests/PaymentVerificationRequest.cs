using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanPay.Domain.Services.Requests
{
    public class PaymentVerificationRequest
    {
        public string MerchantID { get; set; }
        public int Amount { get; set; }
        public string Authority { get; set; }
    }
}
