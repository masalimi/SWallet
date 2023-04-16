using Nop.Core;
using Nop.Plugin.Payments.RayanPay.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanPay.Domain.Data
{
    public class RayanPayServiceProxyTransactionRecord:BaseEntity
    {
		public int OrderId { get; set; }
		public int RefCode { get; set; }
		public RayanPayServiceProxyStateEnum State { get; set; }
        public string Authority { get; set; }
		public string RequestJson { get; set; }
		public string ResponseJson { get; set; }
		public DateTime? RequestDateUtc { get; set; }
		public DateTime? ResponseDateUtc { get; set; }
	}
}

