using Nop.Plugin.Payments.RayanPay.Domain.Services.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanPay.Services
{
	public interface IRayanPayServiceProxy
	{
		Task<HttpResponseMessage> PaymentRequestAsync(string requestUrl, PaymentRequest request);
		Task<HttpResponseMessage> PaymentVerificationAsync(string requestUrl, PaymentVerificationRequest request);
	}
}
