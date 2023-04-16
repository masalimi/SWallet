using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Plugin.Payments.RayanWallet.Data;
using Nop.Plugin.Payments.RayanWallet.Domain.Services;
using Nop.Plugin.Payments.RayanWallet.Domain.Services.Requests;
using Nop.Plugin.Payments.RayanWallet.Services;
using Nop.Plugin.Payments.RayanWallet.Domain.Services.Responses;
using Nop.Services.Common;
using Nop.Services.Customers;
using StackExchange.Profiling.Internal;
using Nop.Plugin.Payments.RayanWallet.Helper;
using Nop.Plugin.Payments.RayanWallet.Domain.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Data;

namespace Nop.Plugin.Payments.RayanWallet
{
    public class RayanWalletPaymentProccessor : BasePlugin, IPaymentMethod
    {
        private readonly ILocalizationService _localizationService;
        private readonly IRayanWalletServiceProxy _rayanWalletServicProxy;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerService _customerService;
        private readonly ISettingService _settingService;
        private readonly IRayanWalletDataService _rayanWalletDataService;
        private readonly RayanWalletObjectContext _objectContext;
        private readonly ILogger _logger;
        private readonly RayanWalletPaymentSettings _rayanWalletPaymentSettings;
        private readonly IWorkContext _workContext;
        private readonly IWalletCustomerHistoryService _walletCustomerHistory;
        private readonly IRayanWalletServiceProxy _walletServiceProxy;
        private readonly IRepository<WalletCustomer> _walletCustomerRepository;

        public RayanWalletPaymentProccessor(ILocalizationService localizationService,
            IRayanWalletServiceProxy rayanWalletServicProxy,
            IWebHelper webHelper,
            ICustomerService customerService,
            ISettingService settingService,
            IRayanWalletDataService rayanWalletDataService,
            RayanWalletObjectContext objectContext,
            ILogger logger,
            RayanWalletPaymentSettings rayanWalletPaymentSettings,
            IWalletCustomerHistoryService walletCustomerHistory,
            IRayanWalletServiceProxy rayanWalletServiceProxy,
            IRepository<WalletCustomer> walletCustomerRepository,
            IWorkContext workContext)
        {
            _localizationService = localizationService;
            _rayanWalletServicProxy = rayanWalletServicProxy;
            _webHelper = webHelper;
            _customerService = customerService;
            _settingService = settingService;
            _rayanWalletDataService = rayanWalletDataService;
            _objectContext = objectContext;
            _logger = logger;
            _rayanWalletPaymentSettings = rayanWalletPaymentSettings;
            _workContext = workContext;
            _walletCustomerHistory = walletCustomerHistory;
            _walletServiceProxy = rayanWalletServiceProxy;
            _walletCustomerRepository = walletCustomerRepository;
        }

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => true;

        public bool SupportRefund => true;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

        public bool SkipPaymentInfo => true;

        public string PaymentMethodDescription
        {
            get
            {
                var currentcustomer = _workContext.CurrentCustomer;
                var walletBalance = _rayanWalletServicProxy.GetBalance(_rayanWalletPaymentSettings.RayanWalletPaymentBaseUrl, currentcustomer).GetAwaiter().GetResult();
                var responseStringBalance = walletBalance.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var balance = JsonConvert.DeserializeObject<WalletBalanceResponse>(responseStringBalance).Balance;
                return _localizationService.GetResource("Plugins.Payments.RayanWallet.PaymentBalanceDescription") + balance;
            }
        }


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
            //viewComponentName = "PaymentRayanWallet";
            throw new NotImplementedException();
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            var currentcustomer = cart[0].Customer;
            //var currentcustomer = cart.Select(p => p.Customer);
            var customerWallet = _rayanWalletServicProxy.CheckCustomerHasWallet(currentcustomer);
            var customerWalletHide = true;
            if (customerWallet == null)
                return customerWalletHide;
            else
            {
                if (customerWallet.Active && string.IsNullOrEmpty(customerWallet.ReferenceAccountId))
                    customerWalletHide = _walletServiceProxy.ActiveWallet(currentcustomer);
                else
                    customerWalletHide = false;
            }
            return customerWalletHide;
        }


        //private void GenerateCustomerWallet(Customer currentcustomer, WalletCustomer walletCustomer)
        //{
        //    try
        //    {
        //        var customer = currentcustomer;
        //        {
        //            var refrenceAccountId = walletCustomer.CustomerId + "_" + customer.Username + "_" + walletCustomer.SourceId + "_" + walletCustomer.StoreId;

        //            #region InsertCreateAccountLog
        //            var createAccountRequest = new WalletCreateAccountRequest()
        //            {
        //                referenceAccountId = refrenceAccountId,
        //                Status = 1,
        //                referenceAccountOwnerId = customer.CustomerGuid.ToString(),
        //                referenceAccountOwnerName = customer.Username,
        //                maxCreditorBalance = walletCustomer.Amount,
        //                maxDebtorBalance = 0,
        //                accountTemplateName = Helper.Constant.KR_User,
        //                referenceAccountTitle = "حساب برداشتی"
        //            };
        //            var WalletServiceProxyTransactionRecord = new Domain.Data.WalletServiceProxyTransactionRecord()
        //            {
        //                OrderId = -1,
        //                RefCode = -1,
        //                State = Domain.Services.RayanWalletServiceProxyStateEnum.createAcount,
        //                RequestJson = JsonConvert.SerializeObject(createAccountRequest, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
        //                RequestDateUtc = DateTime.UtcNow,
        //            };
        //            _rayanWalletDataService.InsertWalletServiceProxyTransactionRecord(WalletServiceProxyTransactionRecord);
        //            #endregion

        //            var walletCreateAccount = _walletServiceProxy.CreateAccount(_rayanWalletPaymentSettings.RayanWalletPaymentBaseUrl, createAccountRequest);
        //            if (walletCreateAccount.Result.Succeeded)
        //            {
        //                #region UpdateWalletCreateAcount
        //                var responseString = walletCreateAccount.Result;

        //                WalletServiceProxyTransactionRecord.ResponseJson = responseString.ToJson();
        //                WalletServiceProxyTransactionRecord.ResponseDateUtc = DateTime.UtcNow;
        //                _rayanWalletDataService.UpdateWalletServiceProxyTransactionRecord(WalletServiceProxyTransactionRecord);
        //                #endregion

        //                #region AddToUserHistory
        //                var walletCustomerHistory = new Domain.Data.WalletCustomerHistory()
        //                {
        //                    CreateDate = DateTime.UtcNow,
        //                    StoreId = customer.RegisteredInStoreId,
        //                    WalletCustomerId = walletCustomer.Id,
        //                    TransactionType = " ایجاد حساب ولت کاربر",
        //                    UpdateDate = DateTime.Now,
        //                    OrderId = Guid.Empty
        //                };
        //                _walletCustomerHistory.InsertWalletCustomerHistory(walletCustomerHistory);
        //                #endregion

        //                var requestDotransaction = new WalletDotransactionRequest()
        //                {
        //                    TransactionType = Helper.Constant.transactionTypeChargeUserWallet,
        //                    Amount = walletCustomer.Amount,
        //                    transactionCreditorAccountItems = new List<AccountItems>
        //                    {
        //                        new AccountItems() {
        //                            amount = walletCustomer.Amount,
        //                            referenceAccountId = refrenceAccountId
        //                        }
        //                    },
        //                    referenceNo = (Constant.ChargeWalletHotKey + customer.Id + "_" + Guid.NewGuid() + "_" + customer.RegisteredInStoreId).ToString(),
        //                    additionalData = customer.Username,
        //                    localDateTime = DateTime.UtcNow,
        //                    Category = "OnlineShop",
        //                    transactionDebtorAccountItems = new List<AccountItems>
        //                {
        //                    new AccountItems(){
        //                        amount = walletCustomer.Amount,
        //                        referenceAccountId = Helper.Constant.KalaresanEntrance
        //                    }
        //                }
        //                };
        //                #region InsertChargeWallet
        //                var WalletServiceProxyTransactionRecordChargeWallet = new Domain.Data.WalletServiceProxyTransactionRecord()
        //                {
        //                    OrderId = -1,
        //                    RefCode = -1,
        //                    State = Domain.Services.RayanWalletServiceProxyStateEnum.chargeWallet,
        //                    RequestJson = JsonConvert.SerializeObject(requestDotransaction, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
        //                    RequestDateUtc = DateTime.UtcNow,
        //                };
        //                _rayanWalletDataService.InsertWalletServiceProxyTransactionRecord(WalletServiceProxyTransactionRecordChargeWallet);
        //                #endregion
        //                var walletDoTransaction = _walletServiceProxy.WalletDoTransaction(_rayanWalletPaymentSettings.RayanWalletPaymentBaseUrl, requestDotransaction);
        //                if (walletDoTransaction.Result.ResponseCode == "00")
        //                {
        //                    walletCustomer.ReferenceAccountId = refrenceAccountId;
        //                    _walletCustomerRepository.Update(walletCustomer);
        //                    #region UpdateWalletCharge
        //                    var response = walletDoTransaction.Result;

        //                    WalletServiceProxyTransactionRecordChargeWallet.ResponseJson = responseString.ToJson();
        //                    WalletServiceProxyTransactionRecordChargeWallet.ResponseDateUtc = DateTime.UtcNow;
        //                    _rayanWalletDataService.UpdateWalletServiceProxyTransactionRecord(WalletServiceProxyTransactionRecordChargeWallet);
        //                    #endregion
        //                    var walletCustomerHistoryCharge = new Domain.Data.WalletCustomerHistory()
        //                    {
        //                        CreateDate = DateTime.UtcNow,
        //                        StoreId = customer.RegisteredInStoreId,
        //                        WalletCustomerId = walletCustomer.Id,
        //                        TransactionType = " شارژ حساب ولت کاربر",
        //                        OrderId = Guid.Empty,
        //                        Amount = walletCustomer.Amount
        //                    };
        //                    _walletCustomerHistory.InsertWalletCustomerHistory(walletCustomerHistoryCharge);
        //                }
        //                else
        //                {
        //                    //_logger.Error($"Error on ActiveWalletInRayanWalletPaymentProccessor: {currentcustomer.Id + "_" + walletCustomer.Id + walletDoTransaction.Result}");
        //                    throw new Exception("doTransaction:" + walletDoTransaction.Result);

        //                }
        //            }
        //            else
        //            {
        //                throw new Exception("createAcount:" + walletCreateAccount.Result);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error($"Error on ActiveWalletInRayanWalletPaymentProccessor: {currentcustomer.Id + "_" + walletCustomer.Id}", ex);
        //        throw ex;
        //    }
        //}

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            try
            {
                var customer = _customerService.GetCustomerById(postProcessPaymentRequest.Order.CustomerId);
                var walletCustomer = _rayanWalletServicProxy.CheckCustomerHasWallet(customer);
                var useToman = _rayanWalletPaymentSettings.RayanWalletPaymentUseToman;
                var baseUrl = _rayanWalletPaymentSettings.RayanWalletPaymentBaseUrl;
                if (walletCustomer == null)
                {
                    _logger.Error(message: $"PostProcessPayment is not Success walletCustomer is null", customer: postProcessPaymentRequest.Order.Customer);
                }
                else
                {
                    #region GetBalance
                    var getBalanceRequest = new WalletBalanceRequest()
                    {
                        referenceAccountId = walletCustomer.ReferenceAccountId,
                    };
                    var walletServiceProxyTransactionRecord = new Domain.Data.WalletServiceProxyTransactionRecord()
                    {
                        OrderId = -1,
                        RefCode = -1,
                        State = Domain.Services.RayanWalletServiceProxyStateEnum.createAcount,
                        RequestJson = JsonConvert.SerializeObject(getBalanceRequest, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                        RequestDateUtc = DateTime.UtcNow,
                    };
                    _rayanWalletDataService.InsertWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecord);
                    #endregion

                    var responseBalance = _rayanWalletServicProxy.GetBalance(baseUrl, customer).GetAwaiter().GetResult();
                    var responseStringBalance = responseBalance.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    #region UpdateGetBalance
                    walletServiceProxyTransactionRecord.ResponseJson = responseStringBalance.ToJson();
                    walletServiceProxyTransactionRecord.ResponseDateUtc = DateTime.UtcNow;
                    _rayanWalletDataService.UpdateWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecord);
                    #endregion

                    var amount = useToman
                        ? (int)Math.Ceiling(postProcessPaymentRequest.Order.OrderTotal * 10)
                        : (int)Math.Ceiling(postProcessPaymentRequest.Order.OrderTotal);

                    var refCode = _rayanWalletPaymentSettings.RayanWalletPaymentRefCode;
                    _rayanWalletPaymentSettings.RayanWalletPaymentRefCode++;
                    _settingService.SaveSetting(_rayanWalletPaymentSettings, postProcessPaymentRequest.Order.StoreId);
                    if (responseBalance.IsSuccessStatusCode)
                    {
                        var orderAmount = JsonConvert.DeserializeObject<WalletBalanceResponse>(responseStringBalance).Balance;
                        if (amount <= orderAmount)
                        {
                            var refNo = (Constant.StorePaymentHotKey + customer.Id + "_" + Guid.NewGuid() + "_" + postProcessPaymentRequest.Order.Id).ToString();
                            var request = new WalletRequest()
                            {
                                Amount = amount,
                                referenceNo = refNo,
                                additionalData = postProcessPaymentRequest.Order.OrderGuid.ToString(),
                                localDateTime = DateTime.UtcNow,
                                transactionCreditorAccountItems =
                                    new List<AccountItems>()
                                    {
                             new AccountItems()
                        {
                            amount = amount,
                            referenceAccountId = Helper.Constant.KalaResanSettlement
                        }
                                    },
                                Category = "OnlineShop",
                                transactionType = Helper.Constant.transactionTypeStorePayment,
                                transactionDebtorAccountItems = new List<AccountItems>()
                    {
                        new AccountItems()
                            {
                                amount = amount,
                                referenceAccountId =walletCustomer.ReferenceAccountId
                            }
                    }
                            };

                            #region InsertRequest
                            var walletServiceProxyTransactionRecordRequest = new Domain.Data.WalletServiceProxyTransactionRecord()
                            {
                                OrderId = postProcessPaymentRequest.Order.Id,
                                RefCode = refCode,
                                State = RayanWalletServiceProxyStateEnum.request,
                                RequestJson = JsonConvert.SerializeObject(request, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                                RequestDateUtc = DateTime.UtcNow,
                            };
                            _rayanWalletDataService.InsertWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecordRequest);
                            #endregion


                            var requestServiceResponse = _rayanWalletServicProxy.WalletRequest(baseUrl, request).GetAwaiter().GetResult();
                            var responseStringRequest = requestServiceResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                            #region UpdateRequest
                            walletServiceProxyTransactionRecordRequest.ResponseJson = responseStringRequest.ToJson();
                            walletServiceProxyTransactionRecordRequest.ResponseDateUtc = DateTime.UtcNow;
                            _rayanWalletDataService.UpdateWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecordRequest);
                            #endregion

                            if (requestServiceResponse.IsSuccessStatusCode)
                            {
                                #region InsertHistoryUser
                                _walletCustomerHistory.InsertWalletCustomerHistory(new WalletCustomerHistory()
                                {
                                    CreateDate = DateTime.UtcNow,
                                    StoreId = customer.RegisteredInStoreId,
                                    WalletCustomerId = walletCustomer.Id,
                                    TransactionType = " درخواست کسر از حساب ولت کاربر",
                                    OrderId = postProcessPaymentRequest.Order.Id,
                                    Amount = amount,
                                    RefNo = refNo
                                });
                                #endregion
                                //responsePaymentResult.NewPaymentStatus = PaymentStatus.Pending;

                                var requestResult = JsonConvert.DeserializeObject<WalletRequestResponse>(responseStringRequest);
                                if (ShowVerifyPopUpSms(customer.Username))
                                {
                                    var walleteVerifyRequest = new VerifyRequest()
                                    {

                                        localDateTime = requestResult.localDateTime,
                                        Amount = amount,
                                        referenceNo = requestResult.referenceNo
                                    };

                                    #region insertVerifyRecord
                                    var walletServiceProxyTransactionRecordVerifyRequest = new Domain.Data.WalletServiceProxyTransactionRecord()
                                    {
                                        OrderId = postProcessPaymentRequest.Order.Id,
                                        RefCode = refCode,
                                        State = RayanWalletServiceProxyStateEnum.verify,
                                        RequestJson = JsonConvert.SerializeObject(walleteVerifyRequest, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                                        RequestDateUtc = DateTime.UtcNow,
                                    };
                                    _rayanWalletDataService.InsertWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecordVerifyRequest);

                                    #endregion
                                    var verify = _rayanWalletServicProxy.WalletVerify(baseUrl, walleteVerifyRequest).GetAwaiter().GetResult();
                                    var responseStringWalletVerify = verify.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                                    #region updateVerifyRecord
                                    walletServiceProxyTransactionRecordVerifyRequest.ResponseJson = responseStringWalletVerify.ToJson();
                                    walletServiceProxyTransactionRecordVerifyRequest.ResponseDateUtc = DateTime.UtcNow;
                                    _rayanWalletDataService.UpdateWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecordVerifyRequest);
                                    #endregion
                                    if (verify.IsSuccessStatusCode)
                                    {
                                        #region InsertHistoryUser
                                        _walletCustomerHistory.InsertWalletCustomerHistory(new WalletCustomerHistory()
                                        {
                                            CreateDate = DateTime.UtcNow,
                                            StoreId = customer.RegisteredInStoreId,
                                            WalletCustomerId = walletCustomer.Id,
                                            TransactionType = " تایید درخواست کسر از حساب ولت کاربر",
                                            OrderId = postProcessPaymentRequest.Order.Id,
                                            Amount = amount,
                                            RefNo = refNo
                                        });
                                        #endregion
                                        postProcessPaymentRequest.Order.PaymentStatus = PaymentStatus.Paid;
                                        var paymentresult = new ProcessPaymentResult
                                        {
                                            NewPaymentStatus = PaymentStatus.Paid
                                        };
                                    }
                                    else
                                    {
                                        _logger.Warning(message: $"WalletVerify is not success", customer: customer);
                                    }

                                }
                                else
                                {
                                    var walletReverseRequest = new ReverseRequest()
                                    {
                                        referenceNo = requestResult.referenceNo,
                                        localDateTime = requestResult.localDateTime,
                                        Amount = amount
                                    };

                                    #region insertReverseRecord
                                    var walletServiceProxyTransactionRecordReverseRequest = new Domain.Data.WalletServiceProxyTransactionRecord()
                                    {
                                        OrderId = postProcessPaymentRequest.Order.Id,
                                        RefCode = refCode,
                                        State = RayanWalletServiceProxyStateEnum.reverse,
                                        RequestJson = JsonConvert.SerializeObject(walletReverseRequest, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                                        RequestDateUtc = DateTime.UtcNow,
                                    };
                                    _rayanWalletDataService.InsertWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecordReverseRequest);
                                    #endregion

                                    var reverseResponse = _rayanWalletServicProxy.WalletReverse(baseUrl, walletReverseRequest).GetAwaiter().GetResult();
                                    var responseStringWalletReverse = reverseResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                                    #region updateReverse
                                    walletServiceProxyTransactionRecordReverseRequest.ResponseJson = responseStringWalletReverse.ToJson();
                                    walletServiceProxyTransactionRecordReverseRequest.ResponseDateUtc = DateTime.UtcNow;
                                    _rayanWalletDataService.UpdateWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecordReverseRequest);
                                    #endregion

                                    #region InsertHistoryUser

                                    #endregion

                                    if (reverseResponse.IsSuccessStatusCode)
                                    {
                                        _walletCustomerHistory.InsertWalletCustomerHistory(new WalletCustomerHistory()
                                        {
                                            CreateDate = DateTime.UtcNow,
                                            StoreId = customer.RegisteredInStoreId,
                                            WalletCustomerId = walletCustomer.Id,
                                            TransactionType = " بازگشت درخواست کسر از حساب ولت کاربر",
                                            OrderId = postProcessPaymentRequest.Order.Id,
                                            Amount = amount,
                                            RefNo = refNo
                                        });
                                    }
                                    else
                                    {
                                        _logger.Warning(message: $"WalletReverse is not success", customer: customer);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var result = new ProcessPaymentResult();
                            result.AddError("موجودی کیف پول شما کافی نمی باشد");
                            result.NewPaymentStatus = PaymentStatus.Voided;
                        };
                    }
                    else
                    {
                        _logger.Warning(message: $"GetBalance is not success", customer: customer);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(message: $"PostProcessPayment is not Success " + ex.Message, customer: postProcessPaymentRequest.Order.Customer);
            }
        }
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult();
        }

        private bool ShowVerifyPopUpSms(string customerUsername)
        {
            var random = new Random();
            int result = random.Next(1000, 10000);
            return true;
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult { Errors = new[] { "Process Recurring not supported" } };
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var customer = _customerService.GetCustomerById(refundPaymentRequest.Order.CustomerId);
            var walletCustomer = CheckCustomerHasWallet(customer);
            var useToman = _rayanWalletPaymentSettings.RayanWalletPaymentUseToman;
            var baseUrl = _rayanWalletPaymentSettings.RayanWalletPaymentBaseUrl;
            var refundePaymentResult = new RefundPaymentResult();
            var reqData = GetRequestData(refundPaymentRequest.Order.Id);// _walletCustomerHistory.GetCustomerWalletRefNo(refundPaymentRequest.Order);
            if (refundPaymentRequest.IsPartialRefund)
            {


                #region GetBalance
                var getBalanceRequest = new WalletBalanceRequest()
                {
                    referenceAccountId = walletCustomer.ReferenceAccountId,
                };
                var walletServiceProxyTransactionRecord = new Domain.Data.WalletServiceProxyTransactionRecord()
                {
                    OrderId = refundPaymentRequest.Order.Id,
                    RefCode = -1,
                    State = Domain.Services.RayanWalletServiceProxyStateEnum.refundPartial,
                    RequestJson = JsonConvert.SerializeObject(getBalanceRequest, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                    RequestDateUtc = DateTime.UtcNow,
                };
                _rayanWalletDataService.InsertWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecord);
                #endregion

                var responseBalance = _rayanWalletServicProxy.GetBalance(baseUrl, customer).GetAwaiter().GetResult();
                var responseStringBalance = responseBalance.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                #region UpdateGetBalance
                walletServiceProxyTransactionRecord.ResponseJson = responseStringBalance.ToJson();
                walletServiceProxyTransactionRecord.ResponseDateUtc = DateTime.UtcNow;
                _rayanWalletDataService.UpdateWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecord);
                #endregion

                var amount = useToman
                    ? (int)Math.Ceiling(refundPaymentRequest.AmountToRefund * 10)
                    : (int)Math.Ceiling(refundPaymentRequest.AmountToRefund);

                var refCode = _rayanWalletPaymentSettings.RayanWalletPaymentRefCode;
                _rayanWalletPaymentSettings.RayanWalletPaymentRefCode++;
                _settingService.SaveSetting(_rayanWalletPaymentSettings, refundPaymentRequest.Order.StoreId);
                if (responseBalance.IsSuccessStatusCode)
                {
                    var orderAmount = JsonConvert.DeserializeObject<WalletBalanceResponse>(responseStringBalance).Balance;
                    if (amount <= orderAmount)
                    {
                        var request = new WalletRequest()
                        {
                            Amount = amount,
                            referenceNo = (Constant.PartailRefundHotKey + customer.Id + "_" + Guid.NewGuid() + "_" + customer.RegisteredInStoreId + "_" + refundPaymentRequest.Order.Id).ToString(),
                            additionalData = refundPaymentRequest.Order.OrderGuid.ToString(),
                            localDateTime = DateTime.UtcNow,
                            transactionCreditorAccountItems =
                                new List<AccountItems>()
                                {
                            new AccountItems()
                            {
                                amount = amount,
                                referenceAccountId =walletCustomer.ReferenceAccountId
                            }
                                },
                            Category = "OnlineShop",
                            transactionType = Helper.Constant.transactionTypeStorePayment,
                            transactionDebtorAccountItems = new List<AccountItems>()
                    {
                         new AccountItems()
                        {
                            amount = amount,
                            referenceAccountId = Helper.Constant.KalaResanSettlement
                        }
                    }
                        };

                        #region InsertRefundRequest
                        var walletServiceProxyTransactionRecordRequest = new Domain.Data.WalletServiceProxyTransactionRecord()
                        {
                            OrderId = refundPaymentRequest.Order.Id,
                            RefCode = refCode,
                            State = RayanWalletServiceProxyStateEnum.refundPartial,
                            RequestJson = JsonConvert.SerializeObject(request, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                            RequestDateUtc = DateTime.UtcNow,
                        };
                        _rayanWalletDataService.InsertWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecordRequest);
                        #endregion


                        var requestServiceResponse = _rayanWalletServicProxy.WalletRequest(baseUrl, request).GetAwaiter().GetResult();
                        var responseStringRequest = requestServiceResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        #region UpdateRequest
                        walletServiceProxyTransactionRecordRequest.ResponseJson = responseStringRequest.ToJson();
                        walletServiceProxyTransactionRecordRequest.ResponseDateUtc = DateTime.UtcNow;
                        _rayanWalletDataService.UpdateWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecordRequest);
                        #endregion

                        if (requestServiceResponse.IsSuccessStatusCode)
                        {
                            #region InsertHistoryUser
                            _walletCustomerHistory.InsertWalletCustomerHistory(new WalletCustomerHistory()
                            {
                                CreateDate = DateTime.UtcNow,
                                StoreId = customer.RegisteredInStoreId,
                                WalletCustomerId = walletCustomer.Id,
                                TransactionType = " درخواست برگشت به حساب ولت کاربر",
                                OrderId = refundPaymentRequest.Order.Id,
                                Amount = amount
                            });
                            #endregion
                            refundePaymentResult.NewPaymentStatus = PaymentStatus.Pending;
                            var requestResult = JsonConvert.DeserializeObject<WalletRequestResponse>(responseStringRequest);
                            if (ShowVerifyPopUpSms(customer.Username))
                            {
                                var walleteVerifyRequest = new VerifyRequest()
                                {

                                    localDateTime = requestResult.localDateTime,
                                    Amount = amount,
                                    referenceNo = requestResult.referenceNo
                                };

                                #region insertVerifyRecord
                                var walletServiceProxyTransactionRecordVerifyRequest = new Domain.Data.WalletServiceProxyTransactionRecord()
                                {
                                    OrderId = refundPaymentRequest.Order.Id,
                                    RefCode = refCode,
                                    State = RayanWalletServiceProxyStateEnum.refundPartialVerify,
                                    RequestJson = JsonConvert.SerializeObject(walleteVerifyRequest, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                                    RequestDateUtc = DateTime.UtcNow,
                                };
                                _rayanWalletDataService.InsertWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecordVerifyRequest);

                                #endregion
                                var verify = _rayanWalletServicProxy.WalletVerify(baseUrl, walleteVerifyRequest).GetAwaiter().GetResult();
                                var responseStringWalletVerify = verify.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                                #region updateVerifyRecord
                                walletServiceProxyTransactionRecordVerifyRequest.ResponseJson = responseStringWalletVerify.ToJson();
                                walletServiceProxyTransactionRecordVerifyRequest.ResponseDateUtc = DateTime.UtcNow;
                                _rayanWalletDataService.UpdateWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecordVerifyRequest);
                                #endregion
                                if (verify.IsSuccessStatusCode)
                                {
                                    #region InsertHistoryUser
                                    _walletCustomerHistory.InsertWalletCustomerHistory(new WalletCustomerHistory()
                                    {
                                        CreateDate = DateTime.UtcNow,
                                        StoreId = customer.RegisteredInStoreId,
                                        WalletCustomerId = walletCustomer.Id,
                                        TransactionType = " تایید درخواست بازگشت به حساب ولت کاربر",
                                        OrderId = refundPaymentRequest.Order.Id,
                                        Amount = amount
                                    });
                                    #endregion
                                    refundePaymentResult.NewPaymentStatus = PaymentStatus.PartiallyRefunded;
                                }
                                else
                                {
                                    _logger.Warning(message: $"WalletRefundVerify is not success", customer: customer);
                                }

                            }
                            else
                            {
                                var walletReverseRequest = new ReverseRequest()
                                {
                                    referenceNo = requestResult.referenceNo,
                                    localDateTime = requestResult.localDateTime,
                                    Amount = amount
                                };

                                #region insertReverseRecord
                                var walletServiceProxyTransactionRecordReverseRequest = new Domain.Data.WalletServiceProxyTransactionRecord()
                                {
                                    OrderId = refundPaymentRequest.Order.Id,
                                    RefCode = refCode,
                                    State = RayanWalletServiceProxyStateEnum.refundPartialReverse,
                                    RequestJson = JsonConvert.SerializeObject(walletReverseRequest, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                                    RequestDateUtc = DateTime.UtcNow,
                                };
                                _rayanWalletDataService.InsertWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecordReverseRequest);
                                #endregion

                                var reverseResponse = _rayanWalletServicProxy.WalletReverse(baseUrl, walletReverseRequest).GetAwaiter().GetResult();
                                var responseStringWalletReverse = reverseResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                                #region updateReverse
                                walletServiceProxyTransactionRecordReverseRequest.ResponseJson = responseStringWalletReverse.ToJson();
                                walletServiceProxyTransactionRecordReverseRequest.ResponseDateUtc = DateTime.UtcNow;
                                _rayanWalletDataService.UpdateWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecordReverseRequest);
                                #endregion

                                #region InsertHistoryUser
                                _walletCustomerHistory.InsertWalletCustomerHistory(new WalletCustomerHistory()
                                {
                                    CreateDate = DateTime.UtcNow,
                                    StoreId = customer.RegisteredInStoreId,
                                    WalletCustomerId = walletCustomer.Id,
                                    TransactionType = " بازگشت درخواست بازگشت به حساب ولت کاربر",
                                    OrderId = refundPaymentRequest.Order.Id,

                                });
                                #endregion

                                if (reverseResponse.IsSuccessStatusCode)
                                {
                                    refundePaymentResult.NewPaymentStatus = PaymentStatus.Voided;
                                }
                                else
                                {
                                    JsonConvert.DeserializeObject<ReverseResponse>(responseStringWalletReverse).errors.ForEach(p => refundePaymentResult.AddError(p.errorDescription));
                                }
                            }
                        }
                    }
                    else
                    {
                        refundePaymentResult.AddError("موجودی کیف پول شما کافی نمی باشد");
                        refundePaymentResult.NewPaymentStatus = PaymentStatus.Voided;
                    };
                }
                else
                {
                    JsonConvert.DeserializeObject<ReverseResponse>(responseStringBalance).errors.ForEach(p => refundePaymentResult.AddError(p.errorDescription));
                    _logger.Warning(message: $"GetBalancePartialRefund is not success", customer: customer);
                }
                return new RefundPaymentResult { Errors = new[] { "PartialRefund Payment not supported" } };
            }
            else
            {
                var refCode = _rayanWalletPaymentSettings.RayanWalletPaymentRefCode;
                _rayanWalletPaymentSettings.RayanWalletPaymentRefCode++;
                _settingService.SaveSetting(_rayanWalletPaymentSettings, refundPaymentRequest.Order.StoreId);
                #region Refund
                var walletRefundRequest = new RefundRequest()
                {
                    originalLocalDateTime = reqData.localDateTime,
                    originalReferenceNo = reqData.referenceNo,
                    refundAdditionalData = refundPaymentRequest.Order.CustomOrderNumber,
                    refundCategory = "OnlineShopRefund",
                    refundLocalDateTime = DateTime.UtcNow,
                    refundReferenceNo = (Constant.RefundHotKey + customer.Id + "_" + Guid.NewGuid() + "_" + refundPaymentRequest.Order.Id).ToString()
                };

                #region insertReFundRecord
                var walletServiceProxyTransactionRecordReverseRequest = new Domain.Data.WalletServiceProxyTransactionRecord()
                {
                    OrderId = refundPaymentRequest.Order.Id,
                    RefCode = refCode,
                    State = RayanWalletServiceProxyStateEnum.refund,
                    RequestJson = JsonConvert.SerializeObject(walletRefundRequest, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                    RequestDateUtc = DateTime.UtcNow,
                };
                _rayanWalletDataService.InsertWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecordReverseRequest);
                #endregion

                var refundResponse = _rayanWalletServicProxy.WalletRefund(baseUrl, walletRefundRequest).GetAwaiter().GetResult();
                var responseStringWalletRefund = refundResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                #region updateReverse
                walletServiceProxyTransactionRecordReverseRequest.ResponseJson = responseStringWalletRefund.ToJson();
                walletServiceProxyTransactionRecordReverseRequest.ResponseDateUtc = DateTime.UtcNow;
                _rayanWalletDataService.UpdateWalletServiceProxyTransactionRecord(walletServiceProxyTransactionRecordReverseRequest);
                #endregion

                #region InsertHistoryUser
                _walletCustomerHistory.InsertWalletCustomerHistory(new WalletCustomerHistory()
                {
                    CreateDate = DateTime.UtcNow,
                    StoreId = customer.RegisteredInStoreId,
                    WalletCustomerId = walletCustomer.Id,
                    TransactionType = " بازگشت درخواست بازگشت به حساب ولت کاربر",
                    OrderId = refundPaymentRequest.Order.Id,

                });
                #endregion

                if (refundResponse.IsSuccessStatusCode)
                {
                    refundePaymentResult.NewPaymentStatus = PaymentStatus.Refunded;
                }
                else
                {
                    JsonConvert.DeserializeObject<ReverseResponse>(responseStringWalletRefund).errors.ForEach(p => refundePaymentResult.AddError(p.errorDescription));
                    refundePaymentResult.NewPaymentStatus = PaymentStatus.Voided;
                }
                #endregion
                return refundePaymentResult;
            }
        }

        private WalletCustomer CheckCustomerHasWallet(Customer customer)
        {
            var walletCustomer = _rayanWalletServicProxy.GetWalletCustomer(customer);
            //if (walletCustomer.IsChange)
            //{
            //    var walletCustomerAmount = _rayanWalletServicProxy.GetWalletCustomerAmount(walletCustomer.Id);
            //    var editWalletBalance = _rayanWalletServicProxy.UpdateWalletBalance(walletCustomer.ReferenceAccountId, walletCustomerAmount.Sum(p => p.Amount));
            //    if (editWalletBalance.IsSuccessStatusCode)
            //    {
            //        //walletCustomer.Amount = walletCustomerAmount.Amount;
            //        walletCustomer.UpdateDate = DateTime.Now;
            //        walletCustomer.IsChange = false;
            //        _walletCustomerRepository.Update(walletCustomer);
            //        return _walletCustomerRepository.GetById(walletCustomer.Id);
            //    }
            //    else
            //    {
            //        _logger.Warning(message: $"CheckCustomerHasWallet is not success", customer: customer);
            //        return walletCustomer;
            //    }
            //}
            //else
            return walletCustomer;
        }

        private WalletRequest GetRequestData(int id)
        {
            var walletTransactionRecord = _rayanWalletDataService.GetServiceTransactionsByOrderId(id).FirstOrDefault();
            return JsonConvert.DeserializeObject<WalletRequest>(walletTransactionRecord.RequestJson);
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
            var settings = new RayanWalletPaymentSettings
            {
                RayanWalletPaymentBaseUrl = "http://ewallet-serviceapi-gateway.staging.rayanpay.local",// "https://pms.rayanpay.com/",
                RayanWalletPaymentAuthorization = " eyJhbGciOiJSUzUxMiIsInR5cCI6ImF0K2p3dCJ9.eyJuYmYiOjE2Njc5ODU0NDcsImV4cCI6MTk4MzYwNDU2NywiaXNzIjoiaHR0cDovL2V3YWxsZXQtd2ViYXBpLWdhdGV3YXkuc3RhZ2luZy5yYXlhbnBheS5sb2NhbCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJUb2tlbjIwMjItMTEtMDkiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjIxM2YxNjZlLWVmZGEtNGY3OS05Zjg3LWIzZDIwOTY4YmY3YiIsInRlbmFudElkIjoiODU0NDRjODItNTU0MS00N2MxLWJjNGEtOGM1NDY0Yjk4MTBhIiwidGVuYW50X25hbWUiOiJLYWxhUmVzYW4iLCJ2YWxpZF9pcHMiOiIiLCJ0b2tlbl90eXBlIjoiU2VydmljZVRva2VuIiwid19zcCI6WyJBY2NvdW50UmVnaXN0cmF0aW9ufHtcInBuXCI6XCJBY2NvdW50UmVnaXN0cmF0aW9uXCIsXCJydlwiOm51bGx9IiwiRG9UcmFuc2FjdGlvbnx7XCJwblwiOlwiRG9UcmFuc2FjdGlvblwiLFwicnZcIjpudWxsfSIsIlZlcmlmeVRyYW5zYWN0aW9ufHtcInBuXCI6XCJWZXJpZnlUcmFuc2FjdGlvblwiLFwicnZcIjpudWxsfSIsIlJldmVyc2VUcmFuc2FjdGlvbnx7XCJwblwiOlwiUmV2ZXJzZVRyYW5zYWN0aW9uXCIsXCJydlwiOm51bGx9IiwiR2V0QmFsYW5jZXx7XCJwblwiOlwiR2V0QmFsYW5jZVwiLFwicnZcIjpudWxsfSIsIlJlZnVuZFRyYW5zYWN0aW9ufHtcInBuXCI6XCJSZWZ1bmRUcmFuc2FjdGlvblwiLFwicnZcIjpudWxsfSIsIkVkaXRBY2NvdW50fHtcInBuXCI6XCJFZGl0QWNjb3VudFwiLFwicnZcIjpudWxsfSIsIkFjY291bnRUZW1wbGF0ZVJlZ2lzdHJhdGlvbnx7XCJwblwiOlwiQWNjb3VudFRlbXBsYXRlUmVnaXN0cmF0aW9uXCIsXCJydlwiOm51bGx9IiwiRWRpdEFjY291bnRUZW1wbGF0ZXx7XCJwblwiOlwiRWRpdEFjY291bnRUZW1wbGF0ZVwiLFwicnZcIjpudWxsfSIsIlRyYW5zYWN0aW9uVHlwZVJlZ2lzdHJhdGlvbnx7XCJwblwiOlwiVHJhbnNhY3Rpb25UeXBlUmVnaXN0cmF0aW9uXCIsXCJydlwiOm51bGx9IiwiRWRpdFRyYW5zYWN0aW9uVHlwZXx7XCJwblwiOlwiRWRpdFRyYW5zYWN0aW9uVHlwZVwiLFwicnZcIjpudWxsfSJdfQ.Gx4-NTYEoUwbWywdxeKLm9jFloXOYXZFD7vByrq3YkqP88G98uuyGKa3kZKFO5H5A9sfUXWX3DEEFuU_sXhApfgW_LgVDj70r5T1R1BrSFQWKncEgtEU8unJiFAaSbqnDK-xkyAtn78TDZkuUKW8kro-rgv1usLaDCBJcoJhYVTU-XCvdY3406npxMWOF4wV2A1xV5y8TcvzgTp0BDrEGFxPo_cIrUiovFiyIeDjSjGbbNXVLi_xoxMKNFx9pgQ9vdBdvnD-KveoL_jdOCI5OB_96KaohMTAKU0iYfiqPKYL5PNLBDvvWtGYyijcP0yH6o0lLzv5VjxsA9ksOGftIg",
                RayanWalletPaymentRefCode = 0,
                RayanWalletPaymentUseToman = true,
            };

            _settingService.SaveSetting(settings);

            _objectContext.Install();
            // _walletCustomerObjectContext.Install();
            //_walletCustomerHistoryObjectContext.Install();

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.RayanWallet.PaymentMethodDescription", "پرداخت از طریق   کیف پول");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.RayanWallet.PaymentBalanceDescription", " موجودی کیف پول شما");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.RayanWallet.OderNo", " شماره سفارش");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.RayanWallet.TransferTypeWallet", " نوع تراکنش کیف پول");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.RayanWallet.Amount", " مبلغ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.RayanWallet.CreateDate", " تاریخ ایجاد");


            base.Install();
        }

        public override void Uninstall()
        {
            _settingService.DeleteSetting<RayanWalletPaymentSettings>();

            _objectContext.Uninstall();
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.RayanWallet.PaymentMethodDescription");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.RayanWallet.PaymentBalanceDescription");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.RayanWallet.OderNo");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.RayanWallet.TransferTypeWallet");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.RayanWallet.Amount");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.RayanWallet.CreateDate");
            base.Uninstall();
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentRayanWallet/Configure";
        }

        public string GetPublicViewComponentName()
        {
            throw new NotImplementedException();
        }
    }
}

