using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.RayanWallet.Domain.Data;
using Nop.Plugin.Payments.RayanWallet.Models;


namespace Nop.Plugin.Payments.RayanWallet.Services
{
    public interface IWalletCustomerHistoryService
    {
        /// <summary>
        /// Logs the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        void Log(WalletCustomerHistory record);

        void InsertWalletCustomerHistory(WalletCustomerHistory walletCustomerHistory);

        //IPagedList<WalletCustomerHistory> FindRecords(string name, string family, string cardNo, string cvv2,
        //    int pageIndex, int pageSize);
        WalletCustomerHistory GetById(int id);
        /// <summary>
        /// Get a shipping by weight record by identifier
        /// </summary>
        /// <param name="WalletCustomerHistoryId">Record identifier</param>
        /// <returns>Shipping by weight record</returns>

        /// <summary>
        /// Update the shipping by weight record
        /// </summary>
        /// <param name="WalletCustomerHistory">Shipping by weight record</param>
        void UpdateWalletCustomerHistory(WalletCustomerHistory WalletCustomerHistory);

        /// <summary>
        /// Delete the shipping by weight record
        /// </summary>
        /// <param name="WalletCustomerHistory">Shipping by weight record</param>
        void DeleteWalletCustomerHistory(WalletCustomerHistory WalletCustomerHistory);
        IPagedList<WalletCustomerHistory> GetAll();
        string GetCustomerWalletRefNo(Order order);
        WalletCustomerHistoryModel PrepareWalletList(int? pageNumber);
    }
}
