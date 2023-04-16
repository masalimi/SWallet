using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanPay.Domain.Services.Responses
{
	public class TokenResponse
	{
        public int Status { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
    }
}
