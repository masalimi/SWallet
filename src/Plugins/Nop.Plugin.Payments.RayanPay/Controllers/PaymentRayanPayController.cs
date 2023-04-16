using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Payments.RayanPay.Models;
//using Nop.Plugin.Payments.RayanPay.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Logging;
using Newtonsoft.Json;
using Nop.Plugin.Payments.RayanPay.Services;
using Nop.Plugin.Payments.RayanPay.Domain.Services.Responses;
using Nop.Plugin.Payments.RayanPay.Domain.Services.Requests;
//using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Services.Messages;
using Nop.Web.Framework.Models.Extensions;
using Nop.Services.Catalog;
//using Nop.Web.Areas.Admin.Factories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Serialization;
//using Nop.Web.Areas.Admin.Helpers;
//using Nop.Plugin.Payments.RayanPay.Domain.Services.Responses;

namespace Nop.Plugin.Payments.RayanPay.Controllers
{
    public class PaymentRayanPayController : BasePaymentController
    {
        #region Properties

        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IRayanPayServiceProxy _rayanPayServicProxy;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILogger _logger;
        private readonly IRayanPayDataService _rayanPayDataService;
        private readonly IStoreContext _storeContext;
        private readonly INotificationService _notificationService;
        private readonly ICategoryService _categoryService;
        //private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        #endregion

        #region ctor

        public PaymentRayanPayController(IPermissionService permissionService,
            IStoreService storeService,
            IWorkContext workContext,
            ISettingService settingService,
            ILocalizationService localizationService,
            IRayanPayServiceProxy rayanPayServicProxy,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            ILogger logger,
            IRayanPayDataService rayanPayDataService,
            IStoreContext storeContext,
            INotificationService notificationService,
            ICategoryService categoryService)
        //IBaseAdminModelFactory baseAdminModelFactory)
        {
            _permissionService = permissionService;
            _storeService = storeService;
            _workContext = workContext;
            _settingService = settingService;
            _localizationService = localizationService;
            _rayanPayServicProxy = rayanPayServicProxy;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _logger = logger;
            _rayanPayDataService = rayanPayDataService;
            _storeContext = storeContext;
            _notificationService = notificationService;
            _categoryService = categoryService;
            //_baseAdminModelFactory = baseAdminModelFactory;
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
            var rayanPayPaymentSettings = _settingService.LoadSetting<RayanPayPaymentSettings>(storeId);

            var model = new ConfigurationModel
            {
                ActiveStoreScopeConfiguration = storeId,
                MerchantId = rayanPayPaymentSettings.RayanPayPaymentMerchantId,
                BaseUrl = rayanPayPaymentSettings.RayanPayPaymentBaseUrl,
                Gateway = rayanPayPaymentSettings.RayanPayPaymentGatewayUrl,
                RefCode = rayanPayPaymentSettings.RayanPayPaymentRefCode,
                UseToman = rayanPayPaymentSettings.RayanPayPaymentUseToman
            };

            if (storeId <= 0)
                return View("~/Plugins/Payments.RayanPay/Views/Configure.cshtml", model);

            model.BaseUrl_OverrideForStore = _settingService.SettingExists(rayanPayPaymentSettings, x => x.RayanPayPaymentBaseUrl, storeId);
            model.Gateway_OverrideForStore = _settingService.SettingExists(rayanPayPaymentSettings, x => x.RayanPayPaymentGatewayUrl, storeId);
            model.RefCode_OverrideForStore = _settingService.SettingExists(rayanPayPaymentSettings, x => x.RayanPayPaymentRefCode, storeId);
            model.UseToman_OverrideForStore = _settingService.SettingExists(rayanPayPaymentSettings, x => x.RayanPayPaymentUseToman, storeId);
            model.MerchantId_OverrideForStore = _settingService.SettingExists(rayanPayPaymentSettings, x => x.RayanPayPaymentMerchantId, storeId);

            return View("~/Plugins/Payments.RayanPay/Views/Configure.cshtml", model);
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
            var rayanPayPaymentSettings = _settingService.LoadSetting<RayanPayPaymentSettings>(storeScope);

            //save settings
            //rayanPayPaymentSettings.RayanPayPaymentAcceptorIp = model.AcceptorIp;
            rayanPayPaymentSettings.RayanPayPaymentBaseUrl = model.BaseUrl;
            rayanPayPaymentSettings.RayanPayPaymentGatewayUrl = model.Gateway;
            rayanPayPaymentSettings.RayanPayPaymentRefCode = model.RefCode;
            rayanPayPaymentSettings.RayanPayPaymentUseToman = model.UseToman;
            rayanPayPaymentSettings.RayanPayPaymentMerchantId = model.MerchantId;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(rayanPayPaymentSettings, x => x.RayanPayPaymentBaseUrl, model.BaseUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(rayanPayPaymentSettings, x => x.RayanPayPaymentGatewayUrl, model.Gateway_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(rayanPayPaymentSettings, x => x.RayanPayPaymentRefCode, model.RefCode_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(rayanPayPaymentSettings, x => x.RayanPayPaymentUseToman, model.UseToman_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(rayanPayPaymentSettings, x => x.RayanPayPaymentMerchantId, model.MerchantId_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion

        #region RayanPay Service CallBack/Verify

        public IActionResult CallBack([FromQuery] string authority, [FromQuery] string status)
        {
            var paymentResponse = new
            {
                Authority = authority,
                Status = status,
            };

            #region Insert Service Transaction

            var rayanPayServiceProxyTransactionRecord = new Domain.Data.RayanPayServiceProxyTransactionRecord
            {
                Authority = authority,
                State = Domain.Services.RayanPayServiceProxyStateEnum.callback,
                ResponseJson = JsonConvert.SerializeObject(paymentResponse, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                ResponseDateUtc = DateTime.UtcNow,
            };

            _rayanPayDataService.InsertRayanPayServiceProxyTransactionRecord(rayanPayServiceProxyTransactionRecord);

            #endregion

            if (status == "OK")
            {
                var rayanPayDataService = _rayanPayDataService.GetServiceTransactionsByAuthority(paymentResponse.Authority).FirstOrDefault(p => p.Authority == paymentResponse.Authority);
                Order order = _orderService.GetOrderByAuthorizationTransactionIdAndPaymentMethod(authority, "Payments.RayanPay");

                try
                {
                    if (order != null)
                    {
                        var setting = _settingService.LoadSetting<RayanPayPaymentSettings>(_storeContext.CurrentStore.Id);
                        var request = new PaymentVerificationRequest()
                        {
                            Authority = paymentResponse.Authority,
                            MerchantID = setting.RayanPayPaymentMerchantId,
                            Amount = setting.RayanPayPaymentUseToman ? (int)Math.Ceiling(order.OrderTotal * 10) : (int)Math.Ceiling(order.OrderTotal)
                        };

                        #region Insert Service Transaction

                        rayanPayServiceProxyTransactionRecord = new Domain.Data.RayanPayServiceProxyTransactionRecord
                        {
                            State = Domain.Services.RayanPayServiceProxyStateEnum.Verification,
                            RequestJson = JsonConvert.SerializeObject(request, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                            RequestDateUtc = DateTime.UtcNow,
                            Authority = paymentResponse.Authority,
                        };

                        _rayanPayDataService.InsertRayanPayServiceProxyTransactionRecord(rayanPayServiceProxyTransactionRecord);

                        #endregion

                        var response = _rayanPayServicProxy.PaymentVerificationAsync(setting.RayanPayPaymentBaseUrl, request).GetAwaiter().GetResult();

                        var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        #region Updeate Service Transaction

                        rayanPayServiceProxyTransactionRecord.ResponseJson = responseString;
                        rayanPayServiceProxyTransactionRecord.ResponseDateUtc = DateTime.UtcNow;
                        _rayanPayDataService.UpdateRayanPayServiceProxyTransactionRecord(rayanPayServiceProxyTransactionRecord);

                        #endregion

                        ;
                        if (response.IsSuccessStatusCode && JsonConvert.DeserializeObject<PaymentVerificationResponse>(response.Content.ReadAsStringAsync()
                                .Result).Status == 100)
                        {
                            _orderProcessingService.MarkOrderAsPaid(order);

                            return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                        }
                        else
                        {
                            if (!_rayanPayDataService.GetServiceTransactionsByOrderId(order.Id).Any(x => x.State == Domain.Services.RayanPayServiceProxyStateEnum.Verification))
                            {
                                CancelOrder(order);
                            }
                            return RedirectToRoute("OrderDetails", new { orderId = order.Id });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("RayanPay Error", ex);
                    CancelOrder(order);
                    return RedirectToRoute("OrderDetails", new { orderId = order.Id });
                }

                return RedirectToRoute("HomePage");
            }

            return RedirectToRoute("HomePage");
        }

        private void CancelOrder(Order order)
        {
            order.PaymentStatus = Nop.Core.Domain.Payments.PaymentStatus.Voided;
            _orderService.UpdateOrder(order);

            if (_orderProcessingService.CanCancelOrder(order))
            {
                _orderProcessingService.CancelOrder(order, false);
            }
        }


        #endregion

    }
}
