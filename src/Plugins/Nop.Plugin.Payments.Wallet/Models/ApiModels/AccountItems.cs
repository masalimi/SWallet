using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.Wallet.Models.ApiModels
{
  public  class AccountItems
    {
        public string referenceAccountId { get; set; }
        public int amount { get; set; }
    }
}
