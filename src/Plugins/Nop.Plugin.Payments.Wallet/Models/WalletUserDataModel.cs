using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Wallet.Models
{
    public class WalletUserDataModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.Payment.Wallet.Fields.CardNo")]
        public string CardNo { get; set; }
        [NopResourceDisplayName("Plugins.Payment.Wallet.Fields.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Plugins.Payment.Wallet.Fields.Family")]
        public string Family { get; set; }
        [NopResourceDisplayName("Plugins.Payment.Wallet.Fields.Cvv2")]
        public string Cvv2 { get; set; }
    }
}

