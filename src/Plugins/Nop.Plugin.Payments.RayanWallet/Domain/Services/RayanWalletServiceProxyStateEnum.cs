using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanWallet.Domain.Services
{
    public enum RayanWalletServiceProxyStateEnum
    {
        createAcount = 0,
        request = 1,
        verify = 2,
        reverse = 3,
        getbalance = 5,
        chargeWallet = 6,
        refundPartial = 7,
        refundPartialVerify = 8,
        refundPartialReverse = 9,
        refund = 10,
        refundVerify = 11,
        refundVerifyReverse = 12,
        EditBalance = 13,
    }
}
