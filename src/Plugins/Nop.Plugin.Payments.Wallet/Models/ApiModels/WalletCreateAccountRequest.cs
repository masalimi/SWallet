using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.Wallet.Models.ApiModels
{
    public class WalletCreateAccountRequest
    {
        public string accountTemplateName { get; set; }
        public string referenceAccountId { get; set; }
        public int maxDebtorBalance { get; set; }
        public int maxCreditorBalance { get; set; }
        public string referenceAccountOwnerId { get; set; }
        public string referenceAccountOwnerName { get; set; }
        public string referenceAccountTitle { get; set; }
        public bool Status { get; set; }
    }
}
