using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Wallet.Models;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.Wallet.Controllers
{
    public class WalletDataController : BasePaymentController
    {
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;


        public WalletDataController(IWebHelper webHelper,
            IWorkContext workContext)
        {
            _webHelper = webHelper;
            _workContext = workContext;
        }
        public IActionResult WalletUserData()
        {
            var model = new WalletUserDataModel();
            var currentcustomer = _workContext.CurrentCustomer.Username;


            return View("~/Plugins/Payments.Wallet/Views/WalletUserDataInfo.cshtml", model);
        }
    }
}
