using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Infrastructure.DAL;
using Superbrands.Libs.DDD.EfCore.Extensions;

namespace Superbrands.Selection.Infrastructure.Configurations
{
    public class SelectionPurchaseSalePointKeyConfiguration : EntityConfigurationBase<SelectionPurchaseSalePointKeyDalDto>
    {
        public override void Configure(EntityTypeBuilder<SelectionPurchaseSalePointKeyDalDto> builder)
        {
            base.Configure(builder);
            builder.Property(b => b.Id).UseHiLo("SelectionPurchaseSalePointKeySeq");
            builder.Property((x => x.EntityModificationInfo)).IsJsonb().IsRequired();
            builder.HasIndex(spd => new { spd.PurchaseKeyId, spd.SelectionId, spd.SalePointId });
            builder.HasIndex(spd => spd.SelectionId);
            builder.HasIndex(spd => spd.PurchaseKeyId);
            builder.HasIndex(spd => spd.SalePointId);
            builder.HasOne(spd => spd.Selection).WithMany(s => s.SelectionPurchaseSalePointKeys).HasForeignKey(s => s.SelectionId);
        }
    }
}
