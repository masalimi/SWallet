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
    public class WalletCustomerHistoryMap : NopEntityTypeConfiguration<WalletCustomerHistory>
    {
        public override void Configure(EntityTypeBuilder<WalletCustomerHistory> builder)
        {
            builder.ToTable("WalletCustomerHistory");
            //Map the primary key
            builder.HasKey(record => record.Id);
            //Map the additional properties
        }
    }
}
