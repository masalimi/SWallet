using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanWallet.Models
{
    public class ConfigurationModel : BaseSearchModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.RayanPay.BaseUrl")]
        public string BaseUrl { get; set; }
        public bool BaseUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.RayanWallet.UseToman")]
        public bool UseToman { get; set; }
        public bool UseToman_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.RayanPay.RefCode")]
        public int RefCode { get; set; }
        public bool RefCode_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.RayanPay.Authorization")]
        public string Authorization { get; set; }
        public bool Authorization_OverrideForStore { get; set; }
    }
}
