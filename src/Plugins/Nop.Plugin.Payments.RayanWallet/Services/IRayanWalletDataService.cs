using Nop.Core;
using Nop.Plugin.Payments.RayanWallet.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanWallet.Services
{
    public partial interface IRayanWalletDataService
    {
		void InsertWalletServiceProxyTransactionRecord(WalletServiceProxyTransactionRecord rayanWalletServiceProxyTransactionRecord);
		void UpdateWalletServiceProxyTransactionRecord(WalletServiceProxyTransactionRecord rayanWalletServiceProxyTransactionRecord);
		IEnumerable<WalletServiceProxyTransactionRecord> GetServiceTransactionsByOrderId(int orderId);
		IEnumerable<WalletServiceProxyTransactionRecord> GetServiceTransactionsByRefCode(int refCode);
		IEnumerable<WalletServiceProxyTransactionRecord> GetServiceTransactionsByAuthority(string authority);

	}
}
