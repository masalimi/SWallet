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
    public class WalletCustomerAmountMap : NopEntityTypeConfiguration<WalletCustomerAmount>
    {
        public override void Configure(EntityTypeBuilder<WalletCustomerAmount> builder)
        {
            builder.ToTable("WalletCustomerAmount");

            builder.HasKey(x => x.Id);
        }
    }
}
