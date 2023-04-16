using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using Nop.Plugin.Payments.RayanPay.Data;
using Nop.Plugin.Payments.RayanPay.Domain.Data;
using Nop.Plugin.Payments.RayanPay.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanPay.Infrastructure
{
	public class DependencyRegister : IDependencyRegistrar
	{
		public int Order => 6;

		public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
		{
			builder.RegisterType<RayanPayServiceProxy>().As<IRayanPayServiceProxy>().InstancePerLifetimeScope();

			builder.RegisterType<RayanPayDataService>().As<IRayanPayDataService>().InstancePerLifetimeScope();

            //data context
            builder.RegisterPluginDataContext<RayanPayObjectContext>("nop_object_context_RayanPay_zip");

			//override required repository with our custom context
			builder.RegisterType<EfRepository<RayanPayServiceProxyTransactionRecord>>()
				.As<IRepository<RayanPayServiceProxyTransactionRecord>>()
				.WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_RayanPay_zip"))
				.InstancePerLifetimeScope();
		}
	}
}
