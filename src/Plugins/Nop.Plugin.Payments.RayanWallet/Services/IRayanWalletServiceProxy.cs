using Nop.Core.Domain.Customers;
using Nop.Plugin.Payments.RayanWallet.Domain.Services.Requests;
using Nop.Plugin.Payments.RayanWallet.Domain.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Payments.RayanWallet.Domain.Data;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Payments.RayanWallet.Services
{
    public interface IRayanWalletServiceProxy
    {
        //Task<HttpResponseMessage> PaymentRequestAsync(string requestUrl, PaymentRequest request);
        //Task<HttpResponseMessage> PaymentVerificationAsync(string requestUrl, PaymentVerificationRequest request);

        bool ActiveWallet(Customer customer);
        Task<HttpResponseMessage> GetBalance(string baseUrl, Customer customer);
        Task<HttpResponseMessage> CreateAccount(string baseUrl, WalletCreateAccountRequest request);
        Task<HttpResponseMessage> WalletRequest(string baseUrl, WalletRequest request);
        Task<HttpResponseMessage> WalletDoTransaction(string baseUrl, WalletDotransactionRequest request);
        Task<HttpResponseMessage> WalletVerify(string baseUrl, VerifyRequest request);
        Task<HttpResponseMessage> WalletReverse(string baseUrl, ReverseRequest request);
        Task<HttpResponseMessage> WalletRefund(string baseUrl, RefundRequest request);
        string GetCustomerWalletRefrenceId(Customer customer);
        WalletCustomer CheckCustomerHasWallet(Customer customer);
        WalletCustomer GetWalletCustomer(Customer customer);
        HttpResponseMessage UpdateWalletBalance(string referenceAccountId, int amount);
        List<WalletCustomerAmount> GetWalletCustomerAmount(int id);
        //string GetCustomerWalletRefNo(Order order);
    }
}
