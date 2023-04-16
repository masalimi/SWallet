using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Payments.Wallet.Models.ApiModels;

namespace Nop.Plugin.Payments.Wallet.Services
{
    public interface IWalletService
    {
        void ActiveWallet(Customer customer);
        Task<WalletBalanceResponse> GetBalance(Customer customer);
        Task<WalletCreateAccountResponse> CreateAccount(WalletCreateAccountRequest request);
        Task<WalletRequestResponse> WalletRequest(WalletRequest request);
        Task<WalletDotransactionResponse> WalletDoTransaction(WalletDotransactionRequest request);
        Task<VerifyResponse> WalletVerify(VerifyRequest request);
        Task<ReverseResponse> WalletReverse(ReverseRequest request);
        string GetCustomerWalletRefrenceId(Customer customer);
    }
}
