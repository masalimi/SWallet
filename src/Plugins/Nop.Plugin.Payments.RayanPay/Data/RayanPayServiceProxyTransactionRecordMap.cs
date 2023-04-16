using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Payments.RayanPay.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.RayanPay.Data
{
    public class RayanPayServiceProxyTransactionRecordMap : NopEntityTypeConfiguration<RayanPayServiceProxyTransactionRecord>
	{
        public override void Configure(EntityTypeBuilder<RayanPayServiceProxyTransactionRecord> builder)
        {
            builder.ToTable("RayanPayServiceTransaction");

            builder.HasKey(x => x.Id);
        }
    }
}
