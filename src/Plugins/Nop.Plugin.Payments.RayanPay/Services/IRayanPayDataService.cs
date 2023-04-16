using Nop.Core;
using Nop.Plugin.Payments.RayanPay.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanPay.Services
{
    public partial interface IRayanPayDataService
    {
		void InsertRayanPayServiceProxyTransactionRecord(RayanPayServiceProxyTransactionRecord rayanPayServiceProxyTransactionRecord);
		void UpdateRayanPayServiceProxyTransactionRecord(RayanPayServiceProxyTransactionRecord rayanPayServiceProxyTransactionRecord);
		IEnumerable<RayanPayServiceProxyTransactionRecord> GetServiceTransactionsByOrderId(int orderId);
		IEnumerable<RayanPayServiceProxyTransactionRecord> GetServiceTransactionsByRefCode(int refCode);
		IEnumerable<RayanPayServiceProxyTransactionRecord> GetServiceTransactionsByAuthority(string authority);

	}
}
