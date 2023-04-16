using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Plugin.Payments.RayanPay.Data;
using Nop.Plugin.Payments.RayanPay.Domain.Services;
using Nop.Plugin.Payments.RayanPay.Services;
using Nop.Plugin.Payments.RayanPay.Domain.Services.Responses;
using Nop.Services.Common;

namespace Nop.Plugin.Payments.RayanPay
{
    public class RayanPayPaymentProccessor : BasePlugin, IPaymentMethod
    {
        private readonly ILocalizationService _localizationService;
        private readonly IRayanPayServiceProxy _rayanPayServicProxy;
        private readonly IWebHelper _webHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISettingService _settingService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IRayanPayDataService _rayanPayDataService;
        private readonly RayanPayObjectContext _objectContext;
        private readonly ILogger _logger;
        private readonly RayanPayPaymentSettings _rayanPayPaymentSettings;
        private readonly ICategoryService _categoryService;

        public RayanPayPaymentProccessor(ILocalizationService localizationService,
            IRayanPayServiceProxy rayanPayServicProxy,
            IWebHelper webHelper,
            IHttpContextAccessor httpContextAccessor,
            ISettingService settingService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IRayanPayDataService rayanPayDataService,
            RayanPayObjectContext objectContext,
            ILogger logger,
            RayanPayPaymentSettings rayanPayPaymentSettings,
            ICategoryService categoryService)
        {
            _localizationService = localizationService;
            _rayanPayServicProxy = rayanPayServicProxy;
            _webHelper = webHelper;
            _httpContextAccessor = httpContextAccessor;
            _settingService = settingService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _rayanPayDataService = rayanPayDataService;
            _objectContext = objectContext;
            _logger = logger;
            _rayanPayPaymentSettings = rayanPayPaymentSettings;
            _categoryService = categoryService;
        }

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool SkipPaymentInfo => true;

        public string PaymentMethodDescription => _localizationService.GetResource("Plugins.Payments.RayanPay.PaymentMethodDescription");

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return new CancelRecurringPaymentResult { Errors = new[] { "Cancel Recurring not supported" } };
        }

        public bool CanRePostProcessPayment(Order order)
        {
            return true;
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            return new CapturePaymentResult { Errors = new[] { "Capture not supported" } };
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return decimal.Zero;
        }

        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            //TODO: by setting
            //viewComponentName = "PaymentRayanPay";
            throw new NotImplementedException();
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var baseUrl = _rayanPayPaymentSettings.RayanPayPaymentBaseUrl;
            var gatewayUrl = _rayanPayPaymentSettings.RayanPayPaymentGatewayUrl;
            //var acceptorIp = _rayanPayPaymentSettings.RayanPayPaymentAcceptorIp;
            var mechantId = _rayanPayPaymentSettings.RayanPayPaymentMerchantId;
            var useToman = _rayanPayPaymentSettings.RayanPayPaymentUseToman;

            var storeLocation = _webHelper.GetStoreLocation();

            try
            {

                var pendingOrderId = _rayanPayDataService.GetServiceTransactionsByOrderId(postProcessPaymentRequest.Order.Id);
                if (pendingOrderId.Count() > 0)
                    return;

                var refCode = _rayanPayPaymentSettings.RayanPayPaymentRefCode;
                _rayanPayPaymentSettings.RayanPayPaymentRefCode++;
                _settingService.SaveSetting(_rayanPayPaymentSettings, postProcessPaymentRequest.Order.StoreId);

                var request = new Domain.Services.Requests.PaymentRequest()
                {
                    Amount = useToman ? (int)Math.Ceiling(postProcessPaymentRequest.Order.OrderTotal * 10) : (int)Math.Ceiling(postProcessPaymentRequest.Order.OrderTotal),
                    CallbackURL = $"{storeLocation}Plugins/PaymentRayanPay/CallBack",
                    Mobile = "",// postProcessPaymentRequest.Order.Customer.Username,
                    Email = postProcessPaymentRequest.Order.Customer.Email,
                    Description = "",
                    MerchantID = mechantId,
                };

                #region Insert Service Transaction

                var rayanPayServiceProxyTransactionRecord = new Domain.Data.RayanPayServiceProxyTransactionRecord
                {
                    OrderId = postProcessPaymentRequest.Order.Id,
                    RefCode = refCode,
                    State = RayanPayServiceProxyStateEnum.Request,
                    RequestJson = JsonConvert.SerializeObject(request, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                    RequestDateUtc = DateTime.UtcNow,
                };

                _rayanPayDataService.InsertRayanPayServiceProxyTransactionRecord(rayanPayServiceProxyTransactionRecord);

                #endregion

                //Request Token
                var response = _rayanPayServicProxy.PaymentRequestAsync(baseUrl, request).Result;

                var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                //var t = httpResponse.Content.ReadAsStringAsync().Result;
                // JsonSerializer.Deserialize<PaymentResponse>(httpResponse.Content.ReadAsStringAsync().Result, jsonOptions);

                #region Updeate Service Transaction

                rayanPayServiceProxyTransactionRecord.ResponseJson = responseString;
                rayanPayServiceProxyTransactionRecord.ResponseDateUtc = DateTime.UtcNow;
                _rayanPayDataService.UpdateRayanPayServiceProxyTransactionRecord(rayanPayServiceProxyTransactionRecord);

                #endregion

                //Save Request result in order record
                //(Optional)
                postProcessPaymentRequest.Order.AuthorizationTransactionResult = response.StatusCode.ToString();
                _orderService.UpdateOrder(postProcessPaymentRequest.Order);

                if (response.IsSuccessStatusCode && JsonConvert.DeserializeObject<PaymentResponse>(response.Content.ReadAsStringAsync().Result).Status == 100)
                {
                    //merchant_token
                    var authority = JsonConvert.DeserializeObject<PaymentResponse>(responseString).Authority;
                    //Save Token in order record
                    //(Optional)
                    postProcessPaymentRequest.Order.AuthorizationTransactionId = authority;
                    _orderService.UpdateOrder(postProcessPaymentRequest.Order);

                    _httpContextAccessor.HttpContext.Response.Redirect($"{gatewayUrl}{authority}");
                }
                else //if request token is not ok
                {
                    _logger.Warning(message: $"RequestPaymentTokenAsync is not success", customer: postProcessPaymentRequest.Order.Customer);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error on PostProcessPayment", ex, postProcessPaymentRequest.Order.Customer);
            }
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult();
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult { Errors = new[] { "Process Recurring not supported" } };
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            return new RefundPaymentResult { Errors = new[] { "Refund Payment not supported" } };
        }

        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            return new List<string>();
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            return new VoidPaymentResult { Errors = new[] { "Void not supported" } };
        }

        public override void Install()
        {
            var settings = new RayanPayPaymentSettings
            {
                RayanPayPaymentAcceptorIp = 0,
                RayanPayPaymentMerchantId = "1641d31e-ff27-46ec-9903-aad99eae826b",
                RayanPayPaymentBaseUrl = "https://pms.rayanpay.com/",
                RayanPayPaymentGatewayUrl = "https://pms.rayanpay.com/pg/startpay/",
                RayanPayPaymentRefCode = 0,
                RayanPayPaymentUseToman = true,
            };

            _settingService.SaveSetting(settings);

            _objectContext.Install();
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.RayanPay.PaymentMethodDescription", "پرداخت از طریق درگاه", "fa-IR");
            base.Install();
        }

        public override void Uninstall()
        {
            _settingService.DeleteSetting<RayanPayPaymentSettings>();

            _objectContext.Uninstall();
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.RayanPay.PaymentMethodDescription");
            base.Uninstall();
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentRayanPay/Configure";
        }

        public string GetPublicViewComponentName()
        {
            throw new NotImplementedException();
        }
    }
}

