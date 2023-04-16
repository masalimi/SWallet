using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Services.Responses
{
    public class WalletBaseReponse
    {
        public bool Succeeded { get; set; }
        public string ResponseCode { get; set; }
        public List<errors> errors { get; set; }
    }
}
public class errors
{
    public string errorCode { get; set; }
    public string errorDescription { get; set; }
}