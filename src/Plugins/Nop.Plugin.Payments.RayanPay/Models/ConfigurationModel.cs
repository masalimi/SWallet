using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanPay.Models
{
    public class ConfigurationModel : BaseSearchModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.RayanPay.BaseUrl")]
        public string BaseUrl { get; set; }
        public bool BaseUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.RayanPay.GatewayUrl")]
        public string Gateway { get; set; }
        public bool Gateway_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.RayanPay.RefCode")]
        public int RefCode { get; set; }
        public bool RefCode_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.RayanPay.UseToman")]
        public bool UseToman { get; set; }
        public bool UseToman_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.RayanPay.MerchantId")]
        public string MerchantId { get; set; }
        public bool MerchantId_OverrideForStore { get; set; }
    }
}
