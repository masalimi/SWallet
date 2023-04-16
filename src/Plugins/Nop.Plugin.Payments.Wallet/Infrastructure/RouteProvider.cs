using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.Wallet.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {

            //routeBuilder.MapRoute("Plugin.Payments.Wallet", "WalletData/WalletUserDataInfo",
            //    new { controller = "WalletData", action = "WalletUserData" });

            routeBuilder.MapRoute("Plugin.Payments.Wallet.WalletData", "Plugins/PaymentWallet/WalletUserData",
                new { controller = "WalletData", action = "WalletUserData" });

            //routeBuilder.MapRoute("Nop.Plugin.Group.CustomerCalender", "Plugins/Name/CustomerCalendar",
            //            new { controller = "CustomCustomer", action = "CustomerCalendar" });

            //routeBuilder.MapRoute("Nop.Plugin.Payments.CustomerCalender", "Plugins/Name/CustomerCalendar",
            //            new { controller = "CustomCustomer", action = "CustomerCalendar" });

        }

        public int Priority { get; } = 1;
    }
}
