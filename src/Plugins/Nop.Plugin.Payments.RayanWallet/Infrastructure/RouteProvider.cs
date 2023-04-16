using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanWallet.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority
        {
            get { return -1; }
        }

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            ////CallBack
            //routeBuilder.MapRoute("Plugin.Payments.RayanWallet.CallBack", "Plugins/PaymentRayanWallet/CallBack",
            //	 new { controller = "PaymentRayanWallet", action = "CallBack" });

            routeBuilder.MapRoute("Plugin.Payments.RayanWallet.CancelCallBack", "Plugins/PaymentRayanWallet/CancelCallBack",
                 new { controller = "PaymentRayanWallet", action = "CancelCallBack" });

            routeBuilder.MapRoute("Plugin.Payments.RayanWallet.WalletCustomerHistory", "Plugins/PaymentRayanWallet/WalletCustomerHistory",
                 new { controller = "PaymentRayanWallet", action = "WalletCustomerHistory" });

            //routeBuilder.MapRoute("Plugin.Payments.RayanWallet.Verify", "Plugins/PaymentRayanWallet/Verify/{orderId}",
            //	 new { controller = "PaymentRayanWallet", action = "Verify" });

            //         routeBuilder.MapRoute("Plugin.Payments.RayanWallet.TempCallBack", "Plugins/PaymentRayanWallet/TempCallBack",
            //              new { controller = "PaymentRayanWallet", action = "TempCallBack" });
            ////GetCallBack
            //routeBuilder.MapRoute("Plugin.Payments.RayanWallet.GetCallBack", "Plugins/PaymentRayanWallet/GetCallBack",
            //	 new { controller = "PaymentRayanWallet", action = "GetCallBack" });


        }
    }
}
