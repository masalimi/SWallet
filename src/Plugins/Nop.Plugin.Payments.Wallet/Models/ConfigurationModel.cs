using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Wallet.Models
{
    public class ConfigurationModel : BaseSearchModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Wallet.Fields.UseToman")]
        public bool UseToman { get; set; }
        public bool UseToman_OverrideForStore { get; set; }
    }
}
