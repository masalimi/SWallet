using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Payments.RayanWallet.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanWallet.Data
{
    public class WalletServiceProxyTransactionRecordMap : NopEntityTypeConfiguration<WalletServiceProxyTransactionRecord>
	{
        public override void Configure(EntityTypeBuilder<WalletServiceProxyTransactionRecord> builder)
        {
            builder.ToTable("WalletServiceProxyTransactionRecord");

            builder.HasKey(x => x.Id);
        }
    }
}
