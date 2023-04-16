using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Services.Responses
{
    public class PaymentResponse
    {
        public int Status { get; set; }
        public string Authority { get; set; }
    }
}
