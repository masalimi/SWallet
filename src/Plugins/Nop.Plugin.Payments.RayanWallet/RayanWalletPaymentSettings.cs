using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanWallet
{
    public class RayanWalletPaymentSettings : ISettings
    {
        public int RayanWalletPaymentRefCode { get; set; }
        public string RayanWalletPaymentBaseUrl { get; set; }
        //public string RayanWalletPaymentGatewayUrl { get; set; }
        public bool RayanWalletPaymentUseToman { get; set; }
        public string RayanWalletPaymentAuthorization { get; set; }
    }
}
