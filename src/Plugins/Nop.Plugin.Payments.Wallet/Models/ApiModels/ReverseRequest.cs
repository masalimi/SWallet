using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.Wallet.Models.ApiModels
{
    public class ReverseRequest
    {
        public string referenceNo { get; set; }
        public DateTime localDateTime { get; set; }
        public int Amount { get; set; }
    }
}
