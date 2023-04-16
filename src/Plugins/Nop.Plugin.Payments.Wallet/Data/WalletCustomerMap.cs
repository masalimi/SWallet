using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping;
using Nop.Plugin.Payments.Wallet.Domain;

namespace Nop.Plugin.Payments.Wallet.Data
{
    public class WalletCustomerMap : NopEntityTypeConfiguration<WalletCustomer>
    {
        public override void Configure(EntityTypeBuilder<WalletCustomer> builder)
        {
            builder.ToTable(nameof(WalletCustomer));
            //Map the primary key
            builder.HasKey(record => record.Id);
            //Map the additional properties
        }
    }
}
