using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Wallet.Models;
using Nop.Plugin.Payments.Wallet.Services;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.Wallet.Controllers
{
    public class WalletCustomerHistoryController : BasePaymentController
    {
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWalletCustomerHistoryService _walletCustomerHistory;


        public WalletCustomerHistoryController(IWebHelper webHelper,
            IWorkContext workContext,
            IWalletCustomerHistoryService walletCustomerHistory)
        {
            _webHelper = webHelper;
            _workContext = workContext;
            _walletCustomerHistory = walletCustomerHistory;
        }
        public IActionResult WalletCustomerList()
        {
            var model = new WalletCustomerHistoryModel();
            _walletCustomerHistory.GetAll();
            return View("~/Plugins/Payments.Wallet/Views/WalletCustomerHistory.cshtml", model);
        }
    }
}
