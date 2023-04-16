using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Services.Responses
{
    public class PaymentVerificationResponse
    {
        public int Status { get; set; }
        public int RefID { get; set; }
    }
}
