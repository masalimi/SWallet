using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Payments.RayanWallet.Domain.Data;
using Nop.Plugin.Payments.RayanWallet.Services;
using Nop.Services.Events;
using Nop.Services.Logging;

namespace Nop.Plugin.Payments.RayanWallet
{
    public class CustomerLoggedinConsumer : IConsumer<CustomerLoggedinEvent>
    {

        private readonly IRayanWalletServiceProxy _walletServiceProxy;
        public CustomerLoggedinConsumer(IRayanWalletServiceProxy walletServiceProxy)
        {
            _walletServiceProxy = walletServiceProxy;
        }
        public void HandleEvent(CustomerLoggedinEvent data)
        {
            _walletServiceProxy.ActiveWallet(data.Customer);
            //try
            //{
            //    var customer = data.Customer;
            //    var walletCustomer = _walletServiceProxy.CheckCustomerHasWallet(customer.Id);
            //    if (walletCustomer != null && string.IsNullOrEmpty(walletCustomer.ReferenceAccountId))
            //    {
            //        var refrenceAccountId = walletCustomer.CustomerId + "_" + customer.Username + "_" + walletCustomer.SourceId + "_" + walletCustomer.StoreId;
            //        #region InsertCreateAccountLog
            //        var createAccountRequest = new WalletCreateAccountRequest()
            //        {
            //            referenceAccountId = refrenceAccountId,
            //            Status = 1,
            //            referenceAccountOwnerId = customer.CustomerGuid.ToString(),
            //            referenceAccountOwnerName = customer.Username,
            //            maxCreditorBalance = walletCustomer.Amount,
            //            maxDebtorBalance = 0,
            //            accountTemplateName = Helper.Constant.KR_User,
            //            referenceAccountTitle = "حساب برداشتی"
            //        };
            //        var rayanwalletServiceProxyTransactionRecord = new Domain.Data.RayanWalletServiceProxyTransactionRecord()
            //        {
            //            OrderId = -1,
            //            RefCode = -1,
            //            State = Domain.Services.RayanWalletServiceProxyStateEnum.createAcount,
            //            RequestJson = JsonConvert.SerializeObject(createAccountRequest, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
            //            RequestDateUtc = DateTime.UtcNow,
            //        };
            //        _rayanWalletDataService.InsertRayanWalletServiceProxyTransactionRecord(rayanwalletServiceProxyTransactionRecord);
            //        #endregion

            //        var response = _walletServiceProxy.CreateAccount(_rayanWalletPaymentSettings.RayanWalletPaymentBaseUrl, createAccountRequest).GetAwaiter().GetResult();
            //        var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            //        #region UpdateWalletCreateAcount
            //        rayanwalletServiceProxyTransactionRecord.ResponseJson = responseString.ToJson();
            //        rayanwalletServiceProxyTransactionRecord.ResponseDateUtc = DateTime.UtcNow;
            //        _rayanWalletDataService.UpdateRayanWalletServiceProxyTransactionRecord(rayanwalletServiceProxyTransactionRecord);
            //        #endregion

            //        if (response.IsSuccessStatusCode)
            //        {

            //            #region AddToUserHistory
            //            var walletCustomerHistory = new Domain.Data.WalletCustomerHistory()
            //            {
            //                CreateDate = DateTime.UtcNow,
            //                StoreId = customer.RegisteredInStoreId,
            //                WalletCustomerId = walletCustomer.Id,
            //                TransactionType = " ایجاد حساب ولت کاربر",
            //                UpdateDate = DateTime.Now,
            //                OrderId = Guid.Empty
            //            };
            //            _walletCustomerHistory.InsertWalletCustomerHistory(walletCustomerHistory);
            //            #endregion

            //            var requestDotransaction = new WalletDotransactionRequest()
            //            {
            //                TransactionType = Helper.Constant.transactionTypeChargeUserWallet,
            //                Amount = walletCustomer.Amount,
            //                transactionCreditorAccountItems = new List<AccountItems>
            //                {
            //                    new AccountItems() {
            //                        amount = walletCustomer.Amount,
            //                        referenceAccountId = refrenceAccountId
            //                    }
            //                },
            //                referenceNo = (Constant.ChargeWalletHotKey + customer.Id + "_" + Guid.NewGuid() + "_" + customer.RegisteredInStoreId).ToString(),
            //                additionalData = customer.Username,
            //                localDateTime = DateTime.UtcNow,
            //                Category = "OnlineShop",
            //                transactionDebtorAccountItems = new List<AccountItems>
            //            {
            //                new AccountItems(){
            //                    amount = walletCustomer.Amount,
            //                    referenceAccountId = Helper.Constant.KalaresanEntrance
            //                }
            //            }
            //            };
            //            #region InsertChargeWallet
            //            var rayanwalletServiceProxyTransactionRecordChargeWallet = new Domain.Data.RayanWalletServiceProxyTransactionRecord()
            //            {
            //                OrderId = -1,
            //                RefCode = -1,
            //                State = Domain.Services.RayanWalletServiceProxyStateEnum.chargeWallet,
            //                RequestJson = JsonConvert.SerializeObject(requestDotransaction, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
            //                RequestDateUtc = DateTime.UtcNow,
            //            };
            //            _rayanWalletDataService.InsertRayanWalletServiceProxyTransactionRecord(rayanwalletServiceProxyTransactionRecordChargeWallet);
            //            #endregion

            //            var responseDoTransaction = _walletServiceProxy.WalletDoTransaction(_rayanWalletPaymentSettings.RayanWalletPaymentBaseUrl, requestDotransaction).GetAwaiter().GetResult();
            //            var responseStringDoTransaction = responseDoTransaction.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            //            #region UpdateWalletCharge
            //            rayanwalletServiceProxyTransactionRecordChargeWallet.ResponseJson = responseStringDoTransaction.ToJson();
            //            rayanwalletServiceProxyTransactionRecordChargeWallet.ResponseDateUtc = DateTime.UtcNow;
            //            _rayanWalletDataService.UpdateRayanWalletServiceProxyTransactionRecord(rayanwalletServiceProxyTransactionRecordChargeWallet);
            //            #endregion

            //            //if (walletDoTransaction.Result.ResponseCode == "00")
            //            if (responseDoTransaction.IsSuccessStatusCode)
            //            {
            //                walletCustomer.ReferenceAccountId = refrenceAccountId;
            //                _walletCustomerRepository.Update(walletCustomer);

            //                var walletCustomerHistoryCharge = new Domain.Data.WalletCustomerHistory()
            //                {
            //                    CreateDate = DateTime.UtcNow,
            //                    StoreId = customer.RegisteredInStoreId,
            //                    WalletCustomerId = walletCustomer.Id,
            //                    TransactionType = " شارژ حساب ولت کاربر",
            //                    OrderId = Guid.Empty,
            //                    Amount = walletCustomer.Amount
            //                };
            //                _walletCustomerHistory.InsertWalletCustomerHistory(walletCustomerHistoryCharge);
            //            }
            //            else
            //            {
            //                _logger.Warning(message: $"DoTRansction On custtomerLoggedinConsumer is not success", customer: data.Customer);
            //            }
            //        }
            //        else
            //        {
            //            _logger.Warning(message: $"CreateAccount On custtomerLoggedinConsumer is not success", customer: data.Customer);

            //        }
            //    }
            //    else
            //    {
            //        throw new Exception("کاربری با این مشخصات وجود ندارد ");

            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.Error($"Error on RayanWallet ActiveWallet: {data}", ex);
            //    throw ex;
            //}
        }
    }
}
