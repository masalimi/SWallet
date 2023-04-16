using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.Wallet.Data;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Payments.Wallet.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring plugin DB context on application startup
    /// </summary>
    public class NopStartup : INopStartup
    {
        /// <summary>
        /// Represents object for the configuring plugin DB context on application startup
        /// </summary>
        public int Order => 201;

        public void Configure(IApplicationBuilder application)
        {
        }
        //add object context
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<WalletUserDataObjectContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServerWithLazyLoading(services);
            });
            services.AddDbContext<WalletCustomerObjectContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServerWithLazyLoading(services);
            });
            services.AddDbContext<WalletCustomerHistoryObjectContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServerWithLazyLoading(services);
            });
            services.AddHttpClient();
            //services.AddHttpClient("AssemblyAIClient", client =>
            //{
            //    client.BaseAddress = new Uri("https://api.assemblyai.com/");
            //    client.DefaultRequestHeaders.Add("Authorization", "YOUR_ASSEMBLY_AI_TOKEN");
            //});
        }
    }
}
