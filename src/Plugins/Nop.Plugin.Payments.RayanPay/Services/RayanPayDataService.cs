using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Plugin.Payments.RayanPay.Domain.Data;

namespace Nop.Plugin.Payments.RayanPay.Services
{
    public class RayanPayDataService : IRayanPayDataService
    {
        private readonly IRepository<RayanPayServiceProxyTransactionRecord> _rayanPayServiceProxyRepository;

        public RayanPayDataService(IRepository<RayanPayServiceProxyTransactionRecord> rayanPayServiceProxyRepository)
        {
            _rayanPayServiceProxyRepository = rayanPayServiceProxyRepository;
        }

        public void InsertRayanPayServiceProxyTransactionRecord(RayanPayServiceProxyTransactionRecord rayanPayServiceProxyTransactionRecord)
        {
            if (rayanPayServiceProxyTransactionRecord == null)
                throw new ArgumentNullException(nameof(rayanPayServiceProxyTransactionRecord));

            _rayanPayServiceProxyRepository.Insert(rayanPayServiceProxyTransactionRecord);
        }

        public void UpdateRayanPayServiceProxyTransactionRecord(RayanPayServiceProxyTransactionRecord rayanPayServiceProxyTransactionRecord)
        {
            if (rayanPayServiceProxyTransactionRecord == null)
                throw new ArgumentNullException(nameof(rayanPayServiceProxyTransactionRecord));

            _rayanPayServiceProxyRepository.Update(rayanPayServiceProxyTransactionRecord);
        }

        public IEnumerable<RayanPayServiceProxyTransactionRecord> GetServiceTransactionsByOrderId(int orderId)
        {
            try
            {
                var transactions = from rayanPayServiceTranscation in _rayanPayServiceProxyRepository.Table
                                   where rayanPayServiceTranscation.OrderId == orderId
                                   select rayanPayServiceTranscation;
                return transactions;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<RayanPayServiceProxyTransactionRecord> GetServiceTransactionsByRefCode(int refCode)
        {
            var transactions = from rayanPayServiceTransactions in _rayanPayServiceProxyRepository.Table
                               where rayanPayServiceTransactions.RefCode == refCode
                               select rayanPayServiceTransactions;

            return transactions;
        }
        public IEnumerable<RayanPayServiceProxyTransactionRecord> GetServiceTransactionsByAuthority(string authority)
        {
            var transactions = from rayanPayServiceTransactions in _rayanPayServiceProxyRepository.Table
                               where rayanPayServiceTransactions.Authority == authority
                               select rayanPayServiceTransactions;

            return transactions;
        }
    }
}
