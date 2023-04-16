using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanPay.Domain.Services
{
    public enum RayanPayServiceProxyStateEnum
    {
        Request = 0,
        startpay = 1,
        callback = 2,
        nok = 3,
        Verification = 4,
    }
}
