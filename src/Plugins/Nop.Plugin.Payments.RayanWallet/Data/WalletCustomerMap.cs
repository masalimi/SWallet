using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping;
using Nop.Plugin.Payments.RayanWallet.Domain.Data;

namespace Nop.Plugin.Payments.RayanWallet.Data
{
    public class WalletCustomerMap : NopEntityTypeConfiguration<WalletCustomer>
    {
        public override void Configure(EntityTypeBuilder<WalletCustomer> builder)
        {
            builder.ToTable("WalletCustomer");

            builder.HasKey(x => x.Id);
        }
    }
}
