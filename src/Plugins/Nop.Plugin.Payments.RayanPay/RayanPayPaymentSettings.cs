using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanPay
{
    public class RayanPayPaymentSettings : ISettings
    {
        public int RayanPayPaymentRefCode { get; set; }
        public string RayanPayPaymentBaseUrl { get; set; }
        public string RayanPayPaymentGatewayUrl { get; set; }
        public bool RayanPayPaymentUseToman { get; set; }
        public string RayanPayPaymentMerchantId { get; set; }
        public long RayanPayPaymentAcceptorIp { get; set; }
    }
}
