using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Plugin.Payments.RayanPay.Domain.Services.Requests;
using Nop.Services.Logging;
using Newtonsoft.Json.Serialization;

namespace Nop.Plugin.Payments.RayanPay.Services
{
	public class RayanPayServiceProxy : IRayanPayServiceProxy
	{
		private readonly ILogger _logger;
		private readonly IHttpClientFactory _clientFactory;
		public RayanPayServiceProxy(ILogger logger,IHttpClientFactory clientFactory)
		{
			_logger = logger;
			_clientFactory = clientFactory;
		}

		public async Task<HttpResponseMessage> PaymentRequestAsync(string requestUrl, PaymentRequest request)
		{
			var x = new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
			var requestString = JsonConvert.SerializeObject(request, x);

			try
			{
				var client = _clientFactory.CreateClient();
				var response = await client.PostAsync($"{requestUrl}api/v2/ipg/paymentrequest", new StringContent(requestString, Encoding.UTF8, "application/json"));
				return response;
			}
			catch (Exception ex)
			{
				_logger.Error($"Error on RayanPay PaymentRequestAsync: {requestUrl} - {requestString}", ex);
				throw ex;
			}
		}

        public async Task<HttpResponseMessage> PaymentVerificationAsync(string requestUrl, PaymentVerificationRequest request)
        {
			var x = new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
			var requestString = JsonConvert.SerializeObject(request, x);

			try
			{
				var client = _clientFactory.CreateClient();
				var response = await client.PostAsync($"{requestUrl}api/v2/ipg/paymentVerification", new StringContent(requestString, Encoding.UTF8, "application/json"));
				return response;
			}
			catch (Exception ex)
			{
				_logger.Error($"Error on RayanPay PaymentVerificationAsync: {requestUrl} - {requestString}", ex);
				throw ex;
			}
		}
	}
}
