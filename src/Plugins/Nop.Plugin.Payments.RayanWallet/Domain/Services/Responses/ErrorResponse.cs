using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Services.Responses
{
    public class ErrorResponse
    {
        public int errorCode { get; set; }
        public string errorDescription { get; set; }
    }
}
