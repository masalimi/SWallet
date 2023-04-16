using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Payments.Wallet.Domain;
using Nop.Plugin.Payments.Wallet.Helper;
using Nop.Plugin.Payments.Wallet.Models.ApiModels;
using Nop.Services.Tasks;
using Task = Nop.Services.Tasks.Task;

namespace Nop.Plugin.Payments.Wallet.Services
{
    public class WalletService : IWalletService
    {
        private readonly IHttpClientFactory _clientFactory;
        private static string basrUrl = "https://rayanpay.wallet.com";
        private readonly IRepository<WalletUserData> _walletUserDataRepository;
        private readonly IRepository<WalletCustomer> _walletCustomerRepository;
        private readonly IWalletCustomerHistoryService _walletCustomerHistory;


        public WalletService(IHttpClientFactory clientFactory, IRepository<WalletUserData> walletUserDataRepository,
            IRepository<WalletCustomer> walletCustomerRepository, IWalletCustomerHistoryService walletCustomerHistory)
        {
            _clientFactory = clientFactory;
            _walletUserDataRepository = walletUserDataRepository;
            _walletCustomerRepository = walletCustomerRepository;
            _walletCustomerHistory = walletCustomerHistory;
        }


        public void ActiveWallet(Customer customer)
        {
            //todo1==>name
            var walletCustomer = _walletCustomerRepository.Table.FirstOrDefault(p => p.CustomerId == customer.Id);
            if (walletCustomer != null)
            {
                var refrenceAccountId = Guid.NewGuid().ToString();
                var walletCreateAccount = CreateAccount(new WalletCreateAccountRequest()
                {
                    referenceAccountId = refrenceAccountId,
                    Status = true,
                    referenceAccountOwnerId = customer.CustomerGuid.ToString(),
                    referenceAccountOwnerName = customer.Username,
                });
                if (walletCreateAccount.Result.StatusCode == "200" && walletCreateAccount.Result.Succeeded)
                {
                    var walletDoTransaction = WalletDoTransaction(new WalletDotransactionRequest()
                    {
                        transactionType = Constant.transactionType,
                        Amount = walletCustomer.Amount,
                        transactionDebtorAccountItems =
                            {
                                new AccountItems()
                                {
                                    amount = walletCustomer.Amount,
                                    referenceAccountId = Constant.BankReferenceAccountId
                                }
                            },
                        referenceNo = customer.Id.ToString(),
                        additionalData = customer.Username,
                        localDateTime = DateTime.UtcNow,
                        Category = "OnlineShop",
                        transactionCreditorAccountItems =
                        {
                            new AccountItems()
                            {
                                amount = walletCustomer.Amount,
                                referenceAccountId = refrenceAccountId
                            }
                        }
                    });
                    if (walletDoTransaction.Result.StatusCode == "200" &&
                        walletDoTransaction.Result.ResponseCode == "00" && walletDoTransaction.Result.Succeeded)
                    {
                        _walletCustomerHistory.InsertWalletCustomerHistory(new WalletCustomerHistory()
                        {
                            CreateDate = DateTime.UtcNow,
                            StoreId = customer.RegisteredInStoreId,
                            WalletCustomerId = walletCustomer.Id
                        });
                        _walletCustomerRepository.Update(new WalletCustomer()
                        {
                            Id = walletCustomer.Id,
                            ReferenceAccountId = refrenceAccountId,
                        });
                    }
                    else
                    {
                        //todo==>ServiceList-Log
                        //walletDoTransaction.Result.error
                    }
                }
                else
                {
                    //todo==>ServiceErrorList-Log
                    //walletDoTransaction.Result.error
                }
            }
        }

        public async Task<WalletBalanceResponse> GetBalance(Customer customer)
        {
            var referenceAccount = GetCustomerWalletRefrenceId(customer);
            var httpClient = _clientFactory.CreateClient("walletBalance");
            using (httpClient)
            {
                httpClient.BaseAddress = new Uri(basrUrl);
                httpClient.DefaultRequestHeaders.Add("Authorization", "YOUR_ASSEMBLY_AI_TOKEN");
                var json = new
                {
                    referenceAccountId = referenceAccount
                };
                var payload = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");
                var httpResponse = await httpClient.PostAsync("/api/v1/Balance/balance",
                    payload);

                if (httpResponse.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<WalletBalanceResponse>(httpResponse.Content.ReadAsStringAsync()
                        .Result);
                }
                //todoErrorMessageLst
                return null;
            }
        }

        public string GetCustomerWalletRefrenceId(Customer customer)
        {
            try
            {
                return _walletCustomerRepository.Table.FirstOrDefault(p => p.CustomerId == customer.Id)?.ReferenceAccountId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<WalletCreateAccountResponse> CreateAccount(WalletCreateAccountRequest request)
        {
            var httpClient = _clientFactory.CreateClient("walletCreateAccount");
            using (httpClient)
            {
                httpClient.BaseAddress = new Uri(basrUrl);
                httpClient.DefaultRequestHeaders.Add("Authorization", "YOUR_ASSEMBLY_AI_TOKEN");

                var json = new
                {
                    accountTemplateName = request.accountTemplateName,
                    referenceAccountId = request.referenceAccountId,
                    maxDebtorBalance = request.maxDebtorBalance,
                    maxCreditorBalance = request.maxCreditorBalance,
                    referenceAccountOwnerId = request.referenceAccountOwnerId,
                    referenceAccountOwnerName = request.referenceAccountOwnerName,
                    referenceAccountTitle = request.referenceAccountTitle,
                    Status = request.Status
                };

                var payload = new StringContent(JsonConvert.SerializeObject(json), Encoding.Unicode, "application/json");

                var httpResponse = await httpClient.PostAsync("/api/v1/AccountManagement/createaccount", payload);

                if (httpResponse.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<WalletCreateAccountResponse>(httpResponse.Content.ReadAsStringAsync()
                        .Result);
                }

                return null;
            }
        }

        public async Task<WalletRequestResponse> WalletRequest(WalletRequest request)
        {
            var httpClient = _clientFactory.CreateClient("walletRequestTransaction");
            using (httpClient)
            {
                httpClient.BaseAddress = new Uri(basrUrl);
                httpClient.DefaultRequestHeaders.Add("Authorization", "YOUR_ASSEMBLY_AI_TOKEN");

                var json = new
                {
                    Amount = request.Amount,
                    Category = request.Category,
                    additionalData = request.additionalData,
                    localDateTime = request.localDateTime,
                    referenceNo = request.referenceNo,
                    transactionCreditorAccountItems = request.transactionCreditorAccountItems,
                    transactionDebtorAccountItems = request.transactionDebtorAccountItems,
                    transactionType = request.transactionType,
                };

                var payload = new StringContent(JsonConvert.SerializeObject(json), Encoding.Unicode, "application/json");

                var httpResponse = await httpClient.PostAsync("/api/v1/Transaction/request", payload);

                if (httpResponse.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<WalletRequestResponse>(httpResponse.Content.ReadAsStringAsync()
                        .Result);
                }
                //todo
                return null;
            }
        }
        public async Task<WalletDotransactionResponse> WalletDoTransaction(WalletDotransactionRequest request)
        {
            var httpClient = _clientFactory.CreateClient("walletDoTransaction");
            using (httpClient)
            {
                httpClient.BaseAddress = new Uri(basrUrl);
                httpClient.DefaultRequestHeaders.Add("Authorization", "YOUR_ASSEMBLY_AI_TOKEN");

                var json = new
                {
                    Amount = request.Amount,
                    Category = request.Category,
                    additionalData = request.additionalData,
                    localDateTime = request.localDateTime,
                    referenceNo = request.referenceNo,
                    transactionCreditorAccountItems = request.transactionCreditorAccountItems,
                    transactionDebtorAccountItems = request.transactionDebtorAccountItems,
                    transactionType = request.transactionType,
                };

                var payload = new StringContent(JsonConvert.SerializeObject(json), Encoding.Unicode, "application/json");

                var httpResponse = await httpClient.PostAsync("/api/v1/Transaction/dotransaction", payload);

                if (httpResponse.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<WalletDotransactionResponse>(httpResponse.Content.ReadAsStringAsync()
                        .Result);
                }
                //todo
                return null;
            }
        }
        public async Task<VerifyResponse> WalletVerify(VerifyRequest request)
        {
            var httpClient = _clientFactory.CreateClient("VerifyTransaction");
            using (httpClient)
            {
                httpClient.BaseAddress = new Uri(basrUrl);
                httpClient.DefaultRequestHeaders.Add("Authorization", "YOUR_ASSEMBLY_AI_TOKEN");

                var json = new
                {
                    Amount = request.Amount,
                    localDateTime = request.localDateTime,
                    referenceNo = request.referenceNo,
                };

                var payload = new StringContent(JsonConvert.SerializeObject(json), Encoding.Unicode, "application/json");

                var httpResponse = await httpClient.PostAsync("/api/v1/Transaction/verify", payload);

                if (httpResponse.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<VerifyResponse>(httpResponse.Content.ReadAsStringAsync()
                        .Result);
                }
                //todo
                return null;
            }
        }
        public async Task<ReverseResponse> WalletReverse(ReverseRequest request)
        {
            var httpClient = _clientFactory.CreateClient("ReverseTransaction");
            using (httpClient)
            {
                httpClient.BaseAddress = new Uri(basrUrl);
                httpClient.DefaultRequestHeaders.Add("Authorization", "YOUR_ASSEMBLY_AI_TOKEN");

                var json = new
                {
                    Amount = request.Amount,
                    localDateTime = request.localDateTime,
                    referenceNo = request.referenceNo,
                };

                var payload = new StringContent(JsonConvert.SerializeObject(json), Encoding.Unicode, "application/json");

                var httpResponse = await httpClient.PostAsync("/api/v1/Transaction/reverse", payload);

                if (httpResponse.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ReverseResponse>(httpResponse.Content.ReadAsStringAsync()
                        .Result);
                }
                //todo
                return null;
            }
        }
    }

}
