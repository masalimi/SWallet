using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using Nop.Plugin.Payments.RayanWallet.Data;
using Nop.Plugin.Payments.RayanWallet.Domain.Data;
using Nop.Plugin.Payments.RayanWallet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanWallet.Infrastructure
{
	public class DependencyRegister : IDependencyRegistrar
	{
		public int Order => 6;

		public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
		{
			builder.RegisterType<RayanWalletServiceProxy>().As<IRayanWalletServiceProxy>().InstancePerLifetimeScope();

			builder.RegisterType<RayanWalletDataService>().As<IRayanWalletDataService>().InstancePerLifetimeScope();

			builder.RegisterType<WalletCustomerHistoryService>().As<IWalletCustomerHistoryService>().InstancePerLifetimeScope();

            //data context
            builder.RegisterPluginDataContext<RayanWalletObjectContext>("nop_object_context_RayanWallet_zip");

			//override required repository with our custom context
			builder.RegisterType<EfRepository<WalletServiceProxyTransactionRecord>>()
				.As<IRepository<WalletServiceProxyTransactionRecord>>()
				.WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_RayanWallet_zip"))
				.InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<WalletCustomer>>()
                .As<IRepository<WalletCustomer>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_RayanWallet_zip"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<WalletCustomerAmount>>()
                .As<IRepository<WalletCustomerAmount>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_RayanWallet_zip"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<WalletCustomerHistory>>()
                .As<IRepository<WalletCustomerHistory>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_RayanWallet_zip"))
                .InstancePerLifetimeScope();
        }
	}
}
