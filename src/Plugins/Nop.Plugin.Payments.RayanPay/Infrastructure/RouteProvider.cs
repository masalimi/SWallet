using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanPay.Infrastructure
{
	public class RouteProvider : IRouteProvider
	{
		public int Priority
		{
			get { return -1; }
		}

		public void RegisterRoutes(IRouteBuilder routeBuilder)
		{
			//CallBack
			routeBuilder.MapRoute("Plugin.Payments.RayanPay.CallBack", "Plugins/PaymentRayanPay/CallBack",
				 new { controller = "PaymentRayanPay", action = "CallBack" });

			routeBuilder.MapRoute("Plugin.Payments.RayanPay.CancelCallBack", "Plugins/PaymentRayanPay/CancelCallBack",
				 new { controller = "PaymentRayanPay", action = "CancelCallBack" });

			//routeBuilder.MapRoute("Plugin.Payments.RayanPay.Verify", "Plugins/PaymentRayanPay/Verify/{orderId}",
			//	 new { controller = "PaymentRayanPay", action = "Verify" });

			//         routeBuilder.MapRoute("Plugin.Payments.RayanPay.TempCallBack", "Plugins/PaymentRayanPay/TempCallBack",
			//              new { controller = "PaymentRayanPay", action = "TempCallBack" });
			////GetCallBack
			//routeBuilder.MapRoute("Plugin.Payments.RayanPay.GetCallBack", "Plugins/PaymentRayanPay/GetCallBack",
			//	 new { controller = "PaymentRayanPay", action = "GetCallBack" });


		}
	}
}
