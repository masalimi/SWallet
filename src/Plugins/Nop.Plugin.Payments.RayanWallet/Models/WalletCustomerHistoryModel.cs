using System.Collections.Generic;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;

namespace Nop.Plugin.Payments.RayanWallet.Models
{
    public class WalletCustomerHistoryModel : BaseNopModel
    {
        public WalletCustomerHistoryModel()
        {
            WalletHistory = new List<WalletHistoryModel>();
        }
        public IList<WalletHistoryModel> WalletHistory { get; set; }
        public PagerModel PagerModel { get; set; }
        //public int RewardPointsBalance { get; set; }
        //public string RewardPointsAmount { get; set; }
        //public int MinimumRewardPointsBalance { get; set; }
        //public string MinimumRewardPointsAmount { get; set; }

        #region Nested classes

        public partial class WalletHistoryModel : BaseNopEntityModel
        {
            #endregion
            [NopResourceDisplayName("Plugins.Payments.RayanWallet.OderId")]
            public int OrderId { get; set; }
            [NopResourceDisplayName("Plugins.Payments.RayanWallet.OrderNo")]
            public string OrderNo { get; set; }
            [NopResourceDisplayName("Plugins.Payments.RayanWallet.Amount")]
            public decimal Amount { get; set; }
            [NopResourceDisplayName("Plugins.Payments.RayanWallet.CreateDate")]
            public string CreateDate { get; set; }
            [NopResourceDisplayName("Plugins.Payments.RayanWallet.UpdateDate")]
            public string UpdateDate { get; set; }
            [NopResourceDisplayName("Plugins.Payments.RayanWallet.TransferTypeWallet")]
            public string TransferTypeWallet { get; set; }
        }
    }

}
