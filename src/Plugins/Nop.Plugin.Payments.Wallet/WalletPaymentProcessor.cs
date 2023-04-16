using System;
using System.Collections.Generic;
using System.Net.Http;

using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Wallet.Data;
using Nop.Plugin.Payments.Wallet.Models.ApiModels;
using Nop.Plugin.Payments.Wallet.Services;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.Wallet
{
    public class WalletPaymentProcessor : BasePlugin, IPaymentMethod
    {
        private static string transactionType = "";
        private static string refrenceIdDeptorCreator = "";
        private readonly ISettingService _settingService;
        private readonly WalletUserDataObjectContext _walletUserDataObjectContext;
        private readonly WalletCustomerHistoryObjectContext _walletCustomerHistoryObjectContext;
        private readonly WalletCustomerObjectContext _walletCustomerObjectContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerService _customerService;
        private readonly IWalletUserDataService _walletUserDataService;
        private readonly IWorkContext _workContext;
        private readonly IWalletService _walletService;
        private readonly IHttpClientFactory _clientFactory;


        public WalletPaymentProcessor(ISettingService settingService,
            WalletUserDataObjectContext walletUserDataObjectContext,
            WalletCustomerHistoryObjectContext walletCustomerHistoryObjectContext,
            WalletCustomerObjectContext walletCustomerObjectContext,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            ICustomerService customerService,
            IWalletUserDataService walletUserDataService,
            IWorkContext workContext,
            IWalletService walletService,
            IHttpClientFactory clientFactory
            )
        {
            _walletUserDataObjectContext = walletUserDataObjectContext;
            _walletCustomerObjectContext = walletCustomerObjectContext;
            _walletCustomerHistoryObjectContext = walletCustomerHistoryObjectContext;
            _settingService = settingService;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _customerService = customerService;
            _walletUserDataService = walletUserDataService;
            _workContext = workContext;
            _walletService = walletService;
            _clientFactory = clientFactory;
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            var orderAmount = _walletService.GetBalance(customer);
            var amount = int.Parse(processPaymentRequest.OrderTotal.ToString());
            var responsePaymentResult = new ProcessPaymentResult();
            if (processPaymentRequest.OrderTotal <= orderAmount.Result.Balance)
            {
                var requestServiceResponse = _walletService.WalletRequest(request: new WalletRequest()
                {
                    Amount = amount,
                    //todocheck
                    referenceNo = processPaymentRequest.CustomerId.ToString(),
                    additionalData = processPaymentRequest.OrderGuid.ToString(),
                    localDateTime = DateTime.UtcNow,
                    transactionCreditorAccountItems = new List<AccountItems>()
                    {
                        new AccountItems()
                         {
                             amount = amount,
                             referenceAccountId = _walletService.GetCustomerWalletRefrenceId(customer)
                         }
                    },
                    Category = "OnlineShop",
                    transactionType = transactionType,
                    transactionDebtorAccountItems = new List<AccountItems>()
                    {
                        new AccountItems()
                         {
                             amount = amount ,
                             referenceAccountId = refrenceIdDeptorCreator
                         }
                    }
                });
                if (requestServiceResponse.Result.ResponseCode == "00" && requestServiceResponse.Result.ResponseCode == "200")
                {
                    responsePaymentResult.NewPaymentStatus = PaymentStatus.Pending;
                    if (ShowVerifyPopUpSms(customer.Username = "09199168566"))
                    {
                        var verify = _walletService.WalletVerify(new VerifyRequest()
                        {
                            localDateTime = requestServiceResponse.Result.localDateTime,
                            Amount = amount,
                            referenceNo = requestServiceResponse.Result.referenceNo
                        });
                        if (verify.Result.Succeeded && verify.Result.StatusCode == "200" &&
                            verify.Result.ResponseCode == "00")
                        {
                            responsePaymentResult.NewPaymentStatus = PaymentStatus.Paid;
                        }
                        else
                        {
                            responsePaymentResult.AddError(verify.Result.errorDescription);
                        }
                    }
                    else
                    {
                        var reverse = _walletService.WalletReverse(new ReverseRequest()
                        {
                            referenceNo = requestServiceResponse.Result.referenceNo,
                            localDateTime = requestServiceResponse.Result.localDateTime,
                            Amount = amount
                        });
                        if (reverse.Result.Succeeded && reverse.Result.StatusCode == "200" &&
                            reverse.Result.ResponseCode == "00")
                        {
                            responsePaymentResult.NewPaymentStatus = PaymentStatus.Voided;
                        }
                        else
                        {
                            responsePaymentResult.AddError(reverse.Result.errorDescription);
                        }

                    }
                }
                return new ProcessPaymentResult() { NewPaymentStatus = PaymentStatus.Paid };
            }
            else
            {
                var result = new ProcessPaymentResult();
                result.AddError("موجودی کیف پول شما کافی نمی باشد");
                result.NewPaymentStatus = PaymentStatus.Voided;
                return result;
            };
        }

        private bool ShowVerifyPopUpSms(string customerUsername)
        {
            var random = new Random();
            int result = random.Next(1000, 10000);
            return true;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            var currentcustomer = _workContext.CurrentCustomer;
            var customerHasWallet = _walletService.GetCustomerWalletRefrenceId(currentcustomer);
            return customerHasWallet == null;
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return 0;
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            return new CapturePaymentResult { Errors = new[] { "Capture method not supported" } };
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            return new RefundPaymentResult { Errors = new[] { "Refund method not supported" } };
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            return new VoidPaymentResult { Errors = new[] { "Void method not supported" } };
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult();
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return new CancelRecurringPaymentResult();
        }

        public bool CanRePostProcessPayment(Order order)
        {
            return false;
        }

        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            return new List<string>();
        }

        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest();
            ;
        }

        public string GetPublicViewComponentName()
        {
            return string.Empty;
        }

        public bool SupportCapture { get; set; } = false;
        public bool SupportPartiallyRefund { get; set; } = false;
        public bool SupportRefund { get; set; } = false;
        public bool SupportVoid { get; set; } = true;
        public RecurringPaymentType RecurringPaymentType
        {
            get { return RecurringPaymentType.Manual; }
        }
        public PaymentMethodType PaymentMethodType
        {
            get { return PaymentMethodType.Standard; }
        }
        public bool SkipPaymentInfo
        {
            get { return true; }
        }

        public string PaymentMethodDescription
        {
            get
            {
                var currentcustomer = _workContext.CurrentCustomer;
                var amount = _walletService.GetBalance(currentcustomer);
                return _localizationService.GetResource("Plugins.Payments.Wallet.PaymentBalanceDescription") + amount;
            }
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentWallet/Configure";
        }
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new WalletPaymentSetting());

            //database objects
            _walletUserDataObjectContext.Install();
            _walletCustomerObjectContext.Install();
            _walletCustomerHistoryObjectContext.Install();
            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Wallet.Fields.UseToman", "UseToman");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Wallet.Instructions", "Instructions");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Wallet.PaymentBalanceDescription", "Your Wallet Balance Is");
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<WalletPaymentSetting>();
            //database objects
            _walletUserDataObjectContext.Uninstall();
            _walletCustomerObjectContext.Uninstall();
            _walletCustomerHistoryObjectContext.Uninstall();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Wallet.Fields.UseToman");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Wallet.Instructions");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Wallet.PaymentBalanceDescription");
            base.Uninstall();
        }


    }
}
