using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Plugin.Payments.Wallet.Domain;

namespace Nop.Plugin.Payments.Wallet.Services
{
    public interface IWalletCustomerService
    {
        /// <summary>
        /// Logs the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        void Log(WalletCustomer record);

        void InsertWalletCustomer(WalletCustomer WalletCustomer);

        //IPagedList<WalletCustomer> FindRecords(string name, string family, string cardNo, string cvv2,
        //    int pageIndex, int pageSize);
        WalletCustomer GetById(int id);

        /// <summary>
        /// Get a shipping by weight record by identifier
        /// </summary>
        /// <param name="WalletCustomerId">Record identifier</param>
        /// <returns>Shipping by weight record</returns>

        /// <summary>
        /// Update the shipping by weight record
        /// </summary>
        /// <param name="WalletCustomer">Shipping by weight record</param>
        void UpdateWalletCustomer(WalletCustomer WalletCustomer);

        /// <summary>
        /// Delete the shipping by weight record
        /// </summary>
        /// <param name="WalletCustomer">Shipping by weight record</param>
        void DeleteWalletCustomer(WalletCustomer WalletCustomer);
        //IPagedList<WalletCustomer> GetAll(int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
