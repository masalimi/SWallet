using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Plugin.Payments.RayanWallet.Domain.Services.Requests;
using Nop.Services.Logging;
using Newtonsoft.Json.Serialization;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Payments.RayanWallet.Domain.Data;
using Nop.Plugin.Payments.RayanWallet.Domain.Services;
using Nop.Plugin.Payments.RayanWallet.Domain.Services.Responses;
using Constant = Nop.Plugin.Payments.RayanWallet.Helper.Constant;
using Exception = System.Exception;
using StackExchange.Profiling.Internal;
using Nop.Core.Domain.Orders;
using Newtonsoft.Json.Linq;

namespace Nop.Plugin.Payments.RayanWallet.Services
{
    public class RayanWalletServiceProxy : IRayanWalletServiceProxy
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _clientFactory;

        private readonly IRepository<WalletCustomer> _walletCustomerRepository;
        private readonly IRepository<WalletCustomerAmount> _wallletCustomerAmountReository;
        private readonly IWalletCustomerHistoryService _walletCustomerHistory;
        private readonly IRayanWalletDataService _rayanWalletDataService;
        private readonly RayanWalletPaymentSettings _rayanWalletPaymentSettings;
        public RayanWalletServiceProxy(ILogger logger, IHttpClientFactory clientFactory, RayanWalletPaymentSettings rayanWalletPaymentSettings,
            IRepository<WalletCustomer> walletCustomerRepository, IRayanWalletDataService rayanWalletDataService, IWalletCustomerHistoryService walletCustomerHistoryService,
            IRepository<WalletCustomerAmount> walletCustomerAmountRepository)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _rayanWalletPaymentSettings = rayanWalletPaymentSettings;
            _walletCustomerRepository = walletCustomerRepository;
            _rayanWalletDataService = rayanWalletDataService;
            _walletCustomerHistory = walletCustomerHistoryService;
            _wallletCustomerAmountReository = walletCustomerAmountRepository;
        }


        public bool ActiveWallet(Customer customer)
        {
            var hideWallet = false;
            try
            {
                var walletCustomer = CheckCustomerHasWallet(customer);
                var walletCustomrAmount = GetWalletCustomerAmount(walletCustomer.Id);
                if (walletCustomer != null)
                {
                    if (string.IsNullOrEmpty(walletCustomer.ReferenceAccountId))
                    {
                        var refrenceAccountId =  customer.Username + "_" + walletCustomer.SourceId + "_" + walletCustomer.StoreId;
                        #region InsertCreateAccountLog
                        var createAccountRequest = new WalletCreateAccountRequest()
                        {
                            referenceAccountId = refrenceAccountId,
                            Status = 1,
                            referenceAccountOwnerId = customer.CustomerGuid.ToString(),
                            referenceAccountOwnerName = customer.Username,
                            maxCreditorBalance = (long)(walletCustomrAmount.Any() ? walletCustomrAmount.Sum(p => p.Amount) : 0),
                            maxDebtorBalance = 0,
                            accountTemplateName = Helper.Constant.KR_User,
                            referenceAccountTitle = "حساب برداشتی"
                        };
                        var rayanwalletServiceProxyTransactionRecord = new Domain.Data.WalletServiceProxyTransactionRecord()
                        {
                            OrderId = -1,
                            RefCode = -1,
                            State = Domain.Services.RayanWalletServiceProxyStateEnum.createAcount,
                            RequestJson = JsonConvert.SerializeObject(createAccountRequest, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                            RequestDateUtc = DateTime.UtcNow,
                        };
                        _rayanWalletDataService.InsertWalletServiceProxyTransactionRecord(rayanwalletServiceProxyTransactionRecord);
                        #endregion

                        var response = CreateAccount(_rayanWalletPaymentSettings.RayanWalletPaymentBaseUrl, createAccountRequest).GetAwaiter().GetResult();
                        var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        #region UpdateWalletCreateAcount
                        rayanwalletServiceProxyTransactionRecord.ResponseJson = responseString.ToJson();
                        rayanwalletServiceProxyTransactionRecord.ResponseDateUtc = DateTime.UtcNow;
                        _rayanWalletDataService.UpdateWalletServiceProxyTransactionRecord(rayanwalletServiceProxyTransactionRecord);
                        #endregion

                        if (response.IsSuccessStatusCode)
                        {
                            walletCustomer.ReferenceAccountId = refrenceAccountId;
                            _walletCustomerRepository.Update(walletCustomer);

                            #region AddToUserHistory
                            var walletCustomerHistory = new Domain.Data.WalletCustomerHistory()
                            {
                                CreateDate = DateTime.UtcNow,
                                StoreId = customer.RegisteredInStoreId,
                                WalletCustomerId = walletCustomer.Id,
                                TransactionType = " ایجاد حساب ولت کاربر",
                                UpdateDate = DateTime.Now,
                                OrderId = 0
                            };
                            _walletCustomerHistory.InsertWalletCustomerHistory(walletCustomerHistory);
                            #endregion
                            ChargeWallet(walletCustomer, walletCustomrAmount, customer, refrenceAccountId);
                        }
                        else
                        {
                            var errorCode = JsonConvert.DeserializeObject<dynamic>(responseString);
                            if (errorCode[0]["errorCode"] == "5026")
                            {
                                walletCustomer.ReferenceAccountId = refrenceAccountId;
                                _walletCustomerRepository.Update(walletCustomer);
                                ChargeWallet(walletCustomer, walletCustomrAmount, customer, refrenceAccountId);
                            }
                            else
                            {
                                _logger.Warning(message: $"CreateAccount On custtomerLoggedinConsumer is not success", customer: customer);
                                hideWallet = true;
                            }
                        }
                    }
                    else
                    {
                        ChargeWallet(walletCustomer, walletCustomrAmount, customer, walletCustomer.ReferenceAccountId);
                    }
                }
                else
                {
                    hideWallet = true;
                    throw new Exception("کاربری با این مشخصات وجود ندارد ");

                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error on RayanWallet ActiveWallet: {customer}", ex);
                hideWallet = true;
            }
            return hideWallet;
        }
        public bool ChargeWallet(WalletCustomer walletCustomer, List<WalletCustomerAmount> walletCustomrAmount, Customer customer, string refrenceAccountId)
        {
            var hideWallet = false;
            foreach (var amountVal in walletCustomrAmount)
            {
                var requestDotransaction = new WalletDotransactionRequest()
                {
                    TransactionType = Helper.Constant.transactionTypeChargeUserWallet,
                    Amount = (int)amountVal.Amount,
                    transactionCreditorAccountItems = new List<AccountItems>
                            {
                                new AccountItems() {
                                    amount = (int)amountVal.Amount,
                                    referenceAccountId = refrenceAccountId
                                }
                            },
                    referenceNo = (Constant.ChargeWalletHotKey + customer.Id + "_" + Guid.NewGuid() + "_" + customer.RegisteredInStoreId).ToString(),
                    additionalData = customer.Username,
                    localDateTime = DateTime.UtcNow,
                    Category = "OnlineShop",
                    transactionDebtorAccountItems = new List<AccountItems>
                        {
                            new AccountItems(){
                                amount = (int)amountVal.Amount,
                                referenceAccountId = Helper.Constant.KalaresanEntrance
                            }
                                }
                };
                #region InsertChargeWallet
                var rayanwalletServiceProxyTransactionRecordChargeWallet = new Domain.Data.WalletServiceProxyTransactionRecord()
                {
                    OrderId = -1,
                    RefCode = -1,
                    State = Domain.Services.RayanWalletServiceProxyStateEnum.chargeWallet,
                    RequestJson = JsonConvert.SerializeObject(requestDotransaction, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                    RequestDateUtc = DateTime.UtcNow,
                };
                _rayanWalletDataService.InsertWalletServiceProxyTransactionRecord(rayanwalletServiceProxyTransactionRecordChargeWallet);
                #endregion

                var responseDoTransaction = WalletDoTransaction(_rayanWalletPaymentSettings.RayanWalletPaymentBaseUrl, requestDotransaction).GetAwaiter().GetResult();
                var responseStringDoTransaction = responseDoTransaction.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                #region UpdateWalletCharge
                rayanwalletServiceProxyTransactionRecordChargeWallet.ResponseJson = responseStringDoTransaction.ToJson();
                rayanwalletServiceProxyTransactionRecordChargeWallet.ResponseDateUtc = DateTime.UtcNow;
                _rayanWalletDataService.UpdateWalletServiceProxyTransactionRecord(rayanwalletServiceProxyTransactionRecordChargeWallet);
                #endregion

                //if (walletDoTransaction.Result.ResponseCode == "00")
                if (responseDoTransaction.IsSuccessStatusCode)
                {
                    amountVal.IsApplied = true;
                    _wallletCustomerAmountReository.Update(amountVal);

                    var walletCustomerHistoryCharge = new Domain.Data.WalletCustomerHistory()
                    {
                        CreateDate = DateTime.UtcNow,
                        StoreId = customer.RegisteredInStoreId,
                        WalletCustomerId = walletCustomer.Id,
                        TransactionType = " شارژ حساب ولت کاربر",
                        OrderId = 0,
                        Amount = amountVal.Amount
                    };
                    _walletCustomerHistory.InsertWalletCustomerHistory(walletCustomerHistoryCharge);

                }
                else
                {
                    _logger.Warning(message: $"DoTRansction On custtomerLoggedinConsumer is not success", customer: customer);
                    hideWallet = true;
                }
            }
            return hideWallet;
        }
        //در ابتدا جدول اصلی walletCustomer چک می شود اگر فعال بود و مقدار (ischange )آن true بود مقدار جدید رو از جدول amount  میگیرد و جایگزین مبلغ جدی می کند در جدول و عبارت ischange هم false می شود.
        public WalletCustomer CheckCustomerHasWallet(Customer customer)
        {
            try
            {
                var walletCutomer = _walletCustomerRepository.Table.FirstOrDefault(p => p.Active && p.Username == customer.Username);
                return walletCutomer;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error on RayanWallet CheckCustomerHasWallet: {customer.Id}", ex);
                return null;
            }
        }

        public HttpResponseMessage UpdateWalletBalance(string referenceAccountId, int amount)
        {
            var editBalancesRequest = new WalletEditBalancesRequest()
            {
                referenceAccountId = referenceAccountId,
                maxCreditorBalance = amount,
                maxDebtorBalance = 1,
            };
            var rayanwalletServiceProxyTransactionRecord = new Domain.Data.WalletServiceProxyTransactionRecord()
            {
                OrderId = -1,
                RefCode = -1,
                State = Domain.Services.RayanWalletServiceProxyStateEnum.EditBalance,
                RequestJson = JsonConvert.SerializeObject(editBalancesRequest, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                RequestDateUtc = DateTime.UtcNow,
            };
            _rayanWalletDataService.InsertWalletServiceProxyTransactionRecord(rayanwalletServiceProxyTransactionRecord);

            var response = UpdateBalanceService(_rayanWalletPaymentSettings.RayanWalletPaymentBaseUrl, editBalancesRequest).GetAwaiter().GetResult();
            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            #region UpdateWalletCreateAcount
            rayanwalletServiceProxyTransactionRecord.ResponseJson = responseString.ToJson();
            rayanwalletServiceProxyTransactionRecord.ResponseDateUtc = DateTime.UtcNow;
            _rayanWalletDataService.UpdateWalletServiceProxyTransactionRecord(rayanwalletServiceProxyTransactionRecord);
            #endregion
            return response;
        }

        public async Task<HttpResponseMessage> UpdateBalanceService(string baseUrl, WalletEditBalancesRequest editBalancesRequest)
        {
            var x = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var requestString = JsonConvert.SerializeObject(editBalancesRequest, x);

            try
            {
                var httpClient = _clientFactory.CreateClient("walletEditBalance");
                httpClient.BaseAddress = new Uri(baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _rayanWalletPaymentSettings.RayanWalletPaymentAuthorization);
                return await httpClient.PostAsync($"{baseUrl}/api/v1/AccountManagement/account/EditMaxCreditorDebtorBalances", new StringContent(requestString, Encoding.UTF8, "application/json"));
            }
            catch (Exception ex)
            {
                _logger.Error($"Error on RayanWallet UpdateBalanceService: {baseUrl} - {editBalancesRequest}", ex);
                throw ex;
            }
        }

        public async Task<HttpResponseMessage> GetBalance(string baseUrl, Customer customer)
        {
            var referenceAccount = GetCustomerWalletRefrenceId(customer);
            try
            {
                var httpClient = _clientFactory.CreateClient("walletBalance");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _rayanWalletPaymentSettings.RayanWalletPaymentAuthorization);
                return await httpClient.GetAsync($"{baseUrl}/api/v1/Transaction/balance?ReferenceAccountId=" + referenceAccount);
                //return JsonConvert.DeserializeObject<HttpResponseMessage>(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error on RayanWallet GetBalance: {baseUrl} - {referenceAccount}", ex);
                throw ex;
            }
        }

        public string GetCustomerWalletRefrenceId(Customer customer)
        {
            try
            {
                return _walletCustomerRepository.Table.FirstOrDefault(p => p.Username == customer.Username)?.ReferenceAccountId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<HttpResponseMessage> CreateAccount(string baseUrl, WalletCreateAccountRequest request)
        {
            {
                var x = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                var requestString = JsonConvert.SerializeObject(request, x);

                try
                {
                    var httpClient = _clientFactory.CreateClient("walletCreateAccount");
                    httpClient.BaseAddress = new Uri(baseUrl);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _rayanWalletPaymentSettings.RayanWalletPaymentAuthorization);
                    return await httpClient.PostAsync($"{baseUrl}/api/v1/AccountManagement/account/create", new StringContent(requestString, Encoding.UTF8, "application/json"));
                    //return JsonConvert.DeserializeObject<HttpResponseMessage>(response.Content.ReadAsStringAsync().Result);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error on RayanWallet CreateAccount: {baseUrl} - {request}", ex);
                    throw ex;
                }
            }
        }

        public async Task<HttpResponseMessage> WalletRequest(string baseUrl, WalletRequest request)
        {
            var x = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var requestString = JsonConvert.SerializeObject(request, x);
            try
            {
                var httpClient = _clientFactory.CreateClient("walletRequestTransaction");
                httpClient.BaseAddress = new Uri(baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _rayanWalletPaymentSettings.RayanWalletPaymentAuthorization);
                return await httpClient.PostAsync($"{baseUrl}/api/v1/Transaction/request", new StringContent(requestString, Encoding.UTF8, "application/json"));
                // return JsonConvert.DeserializeObject<HttpResponseMessage>(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error on RayanWallet WalletRequest: {baseUrl} - {request}", ex);
                throw ex;
            }
        }

        public async Task<HttpResponseMessage> WalletDoTransaction(string baseUrl, WalletDotransactionRequest request)
        {
            var x = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var requestString = JsonConvert.SerializeObject(request, x);

            try
            {
                var httpClient = _clientFactory.CreateClient("dotransaction");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _rayanWalletPaymentSettings.RayanWalletPaymentAuthorization);
                return await httpClient.PostAsync($"{baseUrl}/api/v1/Transaction/dotransaction", new StringContent(requestString, Encoding.UTF8, "application/json"));
                //return JsonConvert.DeserializeObject<WalletDotransactionResponse>(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error on RayanWallet WalletDoTransaction: {baseUrl} - {request}", ex);
                throw ex;
            }

        }
        public async Task<HttpResponseMessage> WalletVerify(string baseUrl, VerifyRequest request)
        {
            var x = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var requestString = JsonConvert.SerializeObject(request, x);
            try
            {
                var httpClient = _clientFactory.CreateClient("VerifyTransaction");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _rayanWalletPaymentSettings.RayanWalletPaymentAuthorization);
                return await httpClient.PostAsync($"{baseUrl}/api/v1/Transaction/verify", new StringContent(requestString, Encoding.UTF8, "application/json"));
                //return JsonConvert.DeserializeObject<VerifyResponse>(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error on RayanWallet WalletVerify: {baseUrl} - {request}", ex);
                throw ex;
            }

        }
        public async Task<HttpResponseMessage> WalletReverse(string baseUrl, ReverseRequest request)
        {

            var x = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var requestString = JsonConvert.SerializeObject(request, x);
            try
            {
                var httpClient = _clientFactory.CreateClient("ReverseTransaction");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _rayanWalletPaymentSettings.RayanWalletPaymentAuthorization);
                return await httpClient.PostAsync($"{baseUrl}/api/v1/Transaction/reverse", new StringContent(requestString, Encoding.UTF8, "application/json"));
                //return JsonConvert.DeserializeObject<ReverseResponse>(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error on RayanWallet WalletReverse: {baseUrl} - {request}", ex);
                throw ex;
            }
        }
        public async Task<HttpResponseMessage> WalletRefund(string baseUrl, RefundRequest request)
        {
            var x = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var requestString = JsonConvert.SerializeObject(request, x);
            try
            {
                var httpClient = _clientFactory.CreateClient("RefundTransaction");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _rayanWalletPaymentSettings.RayanWalletPaymentAuthorization);
                return await httpClient.PostAsync($"{baseUrl}/api/v1/Transaction/refund", new StringContent(requestString, Encoding.UTF8, "application/json"));
                //return JsonConvert.DeserializeObject<ReverseResponse>(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error on RayanWallet WalletRefund: {baseUrl} - {request}", ex);
                throw ex;
            }
        }

        public WalletCustomer GetWalletCustomer(Customer customer)
        {
            try
            {
                var walletCutomer = _walletCustomerRepository.Table.FirstOrDefault(p => p.Active && p.Username == customer.Username);
                return walletCutomer;
            }
            catch (Exception ex)
            {
                _logger.Error(message: $"Error on RayanWallet GetWalletCustomer: ", customer: customer);
                throw ex;
            }
        }
        public List<WalletCustomerAmount> GetWalletCustomerAmount(int walletcustomerId)
        {
            try
            {
                var walletCustomerAmount = _wallletCustomerAmountReository.Table.Where(p => !p.IsApplied && p.WalletCustomerId == walletcustomerId);
                return walletCustomerAmount.ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(message: $"Error on RayanWallet GetWalletCustomerAmount: " + walletcustomerId.ToString(), ex);
                throw;
            }
        }

        //private List<int> GetWalletCustomrAmount(int walletCustomerId)
        //{
        //    var walletCustomerAmountList = _wallletCustomerAmountReository.Table.Where(p => p.WalletCustomerId == walletCustomerId && p.Active);
        //    return walletCustomerAmountList.Any() ? walletCustomerAmountList.Select(p => p.Amount).ToList() : null;
        //}
    }
}