using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.RayanWallet.Helper;
using Nop.Plugin.Payments.RayanWallet.Models;
using Nop.Plugin.Payments.RayanWallet.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
//using Nop.Plugin.Payments.RayanWallet.Services;
using Nop.Services.Logging;
//using Nop.Web.Framework.Kendoui;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;
//using Nop.Web.Areas.Admin.Factories;
//using Nop.Web.Areas.Admin.Helpers;
//using Nop.Plugin.Payments.RayanWallet.Domain.Services.Responses;

namespace Nop.Plugin.Payments.RayanWallet.Controllers
{
    public class PaymentRayanWalletController : BasePaymentController
    {

        #region Properties

        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IRayanWalletServiceProxy _rayanWalletServicProxy;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILogger _logger;
        private readonly IRayanWalletDataService _rayanWalletDataService;
        private readonly IStoreContext _storeContext;
        private readonly INotificationService _notificationService;
        private readonly ICategoryService _categoryService;
        private readonly IWalletCustomerHistoryService _walletCustomerHistory;
        private readonly IRepository<Order> _orderRepository;


        #endregion

        #region ctor

        public PaymentRayanWalletController(IPermissionService permissionService,
            IStoreService storeService,
            IWorkContext workContext,
            ISettingService settingService,
            ILocalizationService localizationService,
            IRayanWalletServiceProxy rayanWalletServicProxy,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            ILogger logger,
            IRayanWalletDataService rayanWalletDataService,
            IStoreContext storeContext,
            INotificationService notificationService,
            ICategoryService categoryService,
            IWalletCustomerHistoryService walletCustomerHistoryService,
            IRepository<Order> orderRepository)
        {
            _permissionService = permissionService;
            _storeService = storeService;
            _workContext = workContext;
            _settingService = settingService;
            _localizationService = localizationService;
            _rayanWalletServicProxy = rayanWalletServicProxy;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _logger = logger;
            _rayanWalletDataService = rayanWalletDataService;
            _storeContext = storeContext;
            _notificationService = notificationService;
            _categoryService = categoryService;
            _walletCustomerHistory = walletCustomerHistoryService;
            _orderRepository = orderRepository;
        }

        #endregion

        #region Configure

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var rayanWalletPaymentSettings = _settingService.LoadSetting<RayanWalletPaymentSettings>(storeId);

            var model = new ConfigurationModel
            {
                ActiveStoreScopeConfiguration = storeId,
                Authorization = rayanWalletPaymentSettings.RayanWalletPaymentAuthorization,
                BaseUrl = rayanWalletPaymentSettings.RayanWalletPaymentBaseUrl,
                RefCode = rayanWalletPaymentSettings.RayanWalletPaymentRefCode,
                UseToman = rayanWalletPaymentSettings.RayanWalletPaymentUseToman
            };


            if (storeId <= 0)
                return View("~/Plugins/Payments.RayanWallet/Views/Configure.cshtml", model);

            model.BaseUrl_OverrideForStore = _settingService.SettingExists(rayanWalletPaymentSettings, x => x.RayanWalletPaymentBaseUrl, storeId);
            model.Authorization_OverrideForStore = _settingService.SettingExists(rayanWalletPaymentSettings, x => x.RayanWalletPaymentAuthorization, storeId);
            model.RefCode_OverrideForStore = _settingService.SettingExists(rayanWalletPaymentSettings, x => x.RayanWalletPaymentRefCode, storeId);
            model.UseToman_OverrideForStore = _settingService.SettingExists(rayanWalletPaymentSettings, x => x.RayanWalletPaymentUseToman, storeId);


            return View("~/Plugins/Payments.RayanWallet/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var rayanWalletPaymentSettings = _settingService.LoadSetting<RayanWalletPaymentSettings>(storeScope);

            //save settings
            //rayanWalletPaymentSettings.RayanWalletPaymentAcceptorIp = model.AcceptorIp;
            rayanWalletPaymentSettings.RayanWalletPaymentBaseUrl = model.BaseUrl;
            rayanWalletPaymentSettings.RayanWalletPaymentRefCode = model.RefCode;
            rayanWalletPaymentSettings.RayanWalletPaymentUseToman = model.UseToman;
            rayanWalletPaymentSettings.RayanWalletPaymentAuthorization = model.Authorization;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(rayanWalletPaymentSettings, x => x.RayanWalletPaymentBaseUrl, model.BaseUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(rayanWalletPaymentSettings, x => x.RayanWalletPaymentAuthorization, model.Authorization_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(rayanWalletPaymentSettings, x => x.RayanWalletPaymentRefCode, model.RefCode_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(rayanWalletPaymentSettings, x => x.RayanWalletPaymentUseToman, model.UseToman_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion

        #region WalletCustomerHistory

        public virtual IActionResult WalletCustomerHistory(int? pageNumber)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();
            var model = _walletCustomerHistory.PrepareWalletList(pageNumber);
            return View("~/Plugins/Payments.RayanWallet/Views/WalletCustomerHistory.cshtml", model);

            ////var records = _walletCustomerHistory.GetAll();

            ////var gridModel = new WalletCustomerHistoryListModel().PrepareToGrid(null, records, () =>
            ////{
            ////    return records.Select(record =>
            ////    {
            ////        var model = new WalletCustomerHistoryModel()
            ////        {
            ////            Id = record.Id,
            ////            Amount = record.Amount ?? 0,
            ////            CreateDate = DateTimeExtentions.ToPersianDateTime(record.CreateDate),
            ////            UpdateDate = DateTimeExtentions.ToPersianDateTime(record.UpdateDate),
            ////            OrderNo = _orderRepository.GetById(record.OrderId)?.CustomOrderNumber,
            ////            TransferTypeWallet = record.TransactionType,
            ////        };

            ////        return model;
            ////    });
            ////});
            ////return Json(gridModel);

            //return View("~/Plugins/Payments.RayanWallet/Views/WalletCustomerHistory.cshtml",gridModel);
        }
        #endregion
    }
}
