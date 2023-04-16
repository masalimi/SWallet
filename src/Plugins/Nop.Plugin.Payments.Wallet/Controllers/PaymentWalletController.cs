using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Wallet.Domain;
using Nop.Plugin.Payments.Wallet.Models;
using Nop.Plugin.Payments.Wallet.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;
using System.Linq;

namespace Nop.Plugin.Payments.Wallet.Controllers
{

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class PaymentWalletController : BasePaymentController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly WalletPaymentSetting _walletPaymentSetting;
        private readonly IWalletUserDataService _walletUserDataService;
        private readonly IPaymentService _paymentService;

        #endregion

        #region Ctor

        public PaymentWalletController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWalletUserDataService walletUserDataService,
            WalletPaymentSetting walletPaymentSetting,
            IPaymentService paymentService
            )
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _walletUserDataService = walletUserDataService;
            _walletPaymentSetting = walletPaymentSetting;
            _paymentService = paymentService;
        }

        #endregion

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var walletPaymentSettings = _settingService.LoadSetting<WalletPaymentSetting>(storeScope);

            var model = new ConfigurationModel
            {
                UseToman = walletPaymentSettings.UseToman,
                ActiveStoreScopeConfiguration = storeScope
            };
            if (storeScope > 0)
            {
                model.UseToman_OverrideForStore = _settingService.SettingExists(walletPaymentSettings, x => x.UseToman, storeScope);
            }

            return View("~/Plugins/Payments.Wallet/Views/Configure.cshtml", model);


        }
        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            //save settings
            _walletPaymentSetting.UseToman = model.UseToman;
            _settingService.SaveSetting(_walletPaymentSetting);
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult WalletPaymentTotalList(ConfigurationModel searchModel, WalletUserDataModel filter)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedDataTablesJson();

            //var records = _shippingByWeightService.GetAll(command.Page - 1, command.PageSize);
            var records = _walletUserDataService.GetAll(
              //pageIndex: searchModel.Page - 1,
              //pageSize: searchModel.PageSize
              );

            var gridModel = new WalletUserDataListModel().PrepareToGrid(searchModel, records, () =>
            {
                return records.Select(record =>
                {
                    var model = new WalletUserDataModel()
                    {
                        Id = record.Id,
                        Name = record.Name,
                        Family = record.Family,
                        CardNo = record.CardNo,
                        Cvv2 = record.Cvv2,
                    };

                    return model;
                });
            });

            return Json(gridModel);
        }
        [HttpPost]
        [AdminAntiForgery]
        public IActionResult DeleteWalletPaymentUser(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var sbw = _walletUserDataService.GetById(id);
            if (sbw != null)
                _walletUserDataService.DeletewalletUserData(sbw);

            return new NullJsonResult();
        }

        public IActionResult EditWalletPaymentUser(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var sbw = _walletUserDataService.GetById(id);
            if (sbw == null)
                //no record found with the specified id
                return RedirectToAction("Configure");

            var model = new WalletUserDataModel()
            {
                Id = sbw.Id,
                Name = sbw.Name,
                Family = sbw.Family,
                CardNo = sbw.CardNo,
                Cvv2 = sbw.Cvv2
            };
            return View("~/Plugins/Payments.Wallet/Views/EditPaymentWallet.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult EditWalletPaymentUser(WalletUserDataModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var sbw = _walletUserDataService.GetById(model.Id);
            if (sbw == null)
                //no record found with the specified id
                return RedirectToAction("Configure");

            sbw.Name = model.Name;
            sbw.CardNo = model.CardNo;
            sbw.Cvv2 = model.Cvv2;
            sbw.Family = model.Family;
            _walletUserDataService.UpdateWalletUserData(sbw);

            ViewBag.RefreshPage = true;

            return View("~/Plugins/Payments.Wallet/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult SaveWalletUserData(WalletUserDataModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            _walletUserDataService.InsertWalletUserData(new WalletUserData
            {
                Family = model.Family,
                CardNo = model.CardNo,
                Name = model.Name,
                Cvv2 = model.Cvv2,
            });

            ViewBag.RefreshPage = true;


            return View("~/Plugins/Payments.Wallet/Views/WalletUserDataInfo.cshtml", model);
        }

        public IActionResult SaveWalletUserData()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new WalletUserDataModel();

            return View("~/Plugins/Payments.Wallet/Views/WalletUserDataInfo.cshtml", model);
        }

        public IActionResult WalletUserData()
        {
            var model = new WalletUserDataModel();

            return View("~/Plugins/Payments.Wallet/Views/WalletUserDataInfo.cshtml", model);
        }
    }

}
