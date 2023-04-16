using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Services.Requests
{
    public class WalletRequest
    {
        public string transactionType { get; set; }
        public string additionalData { get; set; }
        public string referenceNo { get; set; }
        public int Amount { get; set; }
        public string Category { get; set; }
        public DateTime localDateTime { get; set; }
        public List<AccountItems> transactionCreditorAccountItems { get; set; }
        public List<AccountItems> transactionDebtorAccountItems { get; set; }
    }
}
