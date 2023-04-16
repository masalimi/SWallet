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
    public class WalletUserDataMap : NopEntityTypeConfiguration<WalletUserData>
    {
        public override void Configure(EntityTypeBuilder<WalletUserData> builder)
        {
            builder.ToTable(nameof(WalletUserData));
            //Map the primary key
            builder.HasKey(record => record.Id);
            //Map the additional properties
            builder.Property(record => record.CardNo);
            //Avoiding truncation/failure
            //so we set the same max length used in the product tame
            builder.Property(record => record.Cvv2).HasMaxLength(400);
            builder.Property(record => record.Family);
            builder.Property(record => record.Name);
            builder.Property(record => record.UserName);
            builder.Property(record => record.Amount);
            //builder.Property(record => record.CustomerId);
            //builder.Property(record => record.Active);
            //builder.Property(record => record.ReferenceAccountId);
            //builder.Property(record => record.StoreId);
            //builder.Property(record => record.SourceId);
            //builder.Property(record => record.UpdateDate);
            //builder.Property(record => record.CreateDate);
        }
    }
}
