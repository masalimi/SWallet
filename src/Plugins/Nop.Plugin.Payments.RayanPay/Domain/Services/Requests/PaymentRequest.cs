using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanPay.Domain.Services.Requests
{
    public class PaymentRequest
    {
        public string MerchantID { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string CallbackURL { get; set; }
    }
}
