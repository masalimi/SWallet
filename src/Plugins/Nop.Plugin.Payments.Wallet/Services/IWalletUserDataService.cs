using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Plugin.Payments.Wallet.Domain;

namespace Nop.Plugin.Payments.Wallet.Services
{
    public interface IWalletUserDataService
    {
        /// <summary>
        /// Logs the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        void Log(WalletUserData record);

        void InsertWalletUserData(WalletUserData walletUserData);

        IPagedList<WalletUserData> FindRecords(string name, string family, string cardNo, string cvv2,
            int pageIndex, int pageSize);
        WalletUserData GetById(int id);

        /// <summary>
        /// Get a shipping by weight record by identifier
        /// </summary>
        /// <param name="walletUserDataId">Record identifier</param>
        /// <returns>Shipping by weight record</returns>

        /// <summary>
        /// Update the shipping by weight record
        /// </summary>
        /// <param name="walletUserData">Shipping by weight record</param>
        void UpdateWalletUserData(WalletUserData walletUserData);

        /// <summary>
        /// Delete the shipping by weight record
        /// </summary>
        /// <param name="walletUserData">Shipping by weight record</param>
        void DeletewalletUserData(WalletUserData walletUserData);
        IPagedList<WalletUserData> GetAll(int pageIndex = 0, int pageSize = int.MaxValue);

        long GetBalanceByUserName(string userName);
    }
}
