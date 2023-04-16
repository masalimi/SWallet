
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Plugin.Payments.RayanWallet.Domain.Data;

namespace Nop.Plugin.Payments.RayanWallet.Services
{
    public class RayanWalletDataService : IRayanWalletDataService
    {
        private readonly IRepository<WalletServiceProxyTransactionRecord> _rayanWalletServiceProxyRepository;

        public RayanWalletDataService(IRepository<WalletServiceProxyTransactionRecord> rayanWalletServiceProxyRepository)
        {
            _rayanWalletServiceProxyRepository = rayanWalletServiceProxyRepository;
        }

        public void InsertWalletServiceProxyTransactionRecord(WalletServiceProxyTransactionRecord rayanWalletServiceProxyTransactionRecord)
        {
            if (rayanWalletServiceProxyTransactionRecord == null)
                throw new ArgumentNullException(nameof(rayanWalletServiceProxyTransactionRecord));

            _rayanWalletServiceProxyRepository.Insert(rayanWalletServiceProxyTransactionRecord);
        }

        public void UpdateWalletServiceProxyTransactionRecord(WalletServiceProxyTransactionRecord rayanWalletServiceProxyTransactionRecord)
        {
            if (rayanWalletServiceProxyTransactionRecord == null)
                throw new ArgumentNullException(nameof(rayanWalletServiceProxyTransactionRecord));

            _rayanWalletServiceProxyRepository.Update(rayanWalletServiceProxyTransactionRecord);
        }

        public IEnumerable<WalletServiceProxyTransactionRecord> GetServiceTransactionsByOrderId(int orderId)
        {
            try
            {
                var transactions = from rayanWalletServiceTranscation in _rayanWalletServiceProxyRepository.Table
                                   where rayanWalletServiceTranscation.OrderId == orderId
                                   select rayanWalletServiceTranscation;
                return transactions;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<WalletServiceProxyTransactionRecord> GetServiceTransactionsByRefCode(int refCode)
        {
            var transactions = from rayanWalletServiceTransactions in _rayanWalletServiceProxyRepository.Table
                               where rayanWalletServiceTransactions.RefCode == refCode
                               select rayanWalletServiceTransactions;

            return transactions;
        }
        public IEnumerable<WalletServiceProxyTransactionRecord> GetServiceTransactionsByAuthority(string authority)
        {
            var transactions = from rayanWalletServiceTransactions in _rayanWalletServiceProxyRepository.Table
                               where rayanWalletServiceTransactions.Authority == authority
                               select rayanWalletServiceTransactions;

            return transactions;
        }
    }
}
