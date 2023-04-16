using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Payments.Wallet.Controllers;
using Nop.Plugin.Payments.Wallet.Data;
using Nop.Plugin.Payments.Wallet.Domain;
using Nop.Plugin.Payments.Wallet.Services;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Payments.Wallet.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_wallet_user_data";
        private const string CONTEXT_NAME_Customer = "nop_object_context_wallet_Customer";
        private const string CONTEXT_NAME_Customer_History = "nop_object_context_wallet_Customer";

        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //builder.RegisterType<WalletUserDataService>().As<IWalletUserDataService>().InstancePerLifetimeScope();
            //builder.RegisterPluginDataContext<WalletUserDataObjectContext>(CONTEXT_NAME);
            //builder.RegisterType<EfRepository<WalletUserData>>()
            //    .As<IRepository<WalletUserData>>()
            //    .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
            //    .InstancePerLifetimeScope();

            builder.RegisterType<WalletCustomerService>().As<IWalletCustomerService>().InstancePerLifetimeScope();
            builder.RegisterPluginDataContext<WalletCustomerObjectContext>(CONTEXT_NAME_Customer);
            builder.RegisterType<EfRepository<WalletCustomer>>().As<IRepository<WalletCustomer>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME_Customer))
                .InstancePerLifetimeScope();

            builder.RegisterType<WalletCustomerHistoryService>().As<IWalletCustomerHistoryService>().InstancePerLifetimeScope();
            builder.RegisterPluginDataContext<WalletCustomerHistoryObjectContext>(CONTEXT_NAME_Customer_History);
            builder.RegisterType<EfRepository<WalletCustomerHistory>>().As<IRepository<WalletCustomerHistory>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME_Customer_History))
                .InstancePerLifetimeScope();
            //data context
            //override required repository with our custom context
        }

        public int Order => 1;
    }
}
