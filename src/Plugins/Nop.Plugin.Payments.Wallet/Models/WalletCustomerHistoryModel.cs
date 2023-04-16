using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Wallet.Models
{
    public class WalletCustomerHistoryModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.Payments.RayanWallet.OderNo")]
        public string OrderNo { get; set; }
        [NopResourceDisplayName("Plugins.Payments.RayanWallet.StoreName")]
        public int StoreName { get; set; }
        [NopResourceDisplayName("Plugins.Payments.RayanWallet.Amount")]
        public int Amount { get; set; }
        [NopResourceDisplayName("Plugins.Payments.RayanWallet.CreateDate")]
        public DateTime CreateDate { get; set; }
        [NopResourceDisplayName("Plugins.Payments.RayanWallet.UpdateDate")]
        public DateTime UpdateDate { get; set; }
        [NopResourceDisplayName("Plugins.Payments.RayanWallet.TransferTypeWallet")]
        public string TransferTypeWallet { get; set; }

    }

}
