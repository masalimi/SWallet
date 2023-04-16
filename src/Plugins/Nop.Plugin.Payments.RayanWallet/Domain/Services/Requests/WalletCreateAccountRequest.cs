using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Services.Requests
{
    public class WalletCreateAccountRequest
    {
        public string accountTemplateName { get; set; }
        public string referenceAccountId { get; set; }
        public long maxDebtorBalance { get; set; }
        public long maxCreditorBalance { get; set; }
        public string referenceAccountOwnerId { get; set; }
        public string referenceAccountOwnerName { get; set; }
        public string referenceAccountTitle { get; set; }
        public int Status { get; set; }
    }
}
