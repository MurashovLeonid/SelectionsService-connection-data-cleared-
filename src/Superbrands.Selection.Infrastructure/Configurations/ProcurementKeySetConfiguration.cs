using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Infrastructure.Configurations
{
    public class ProcurementKeySetConfiguration : EntityConfigurationBase<ProcurementKeySetDalDto>
    {
        public override void Configure(EntityTypeBuilder<ProcurementKeySetDalDto> builder)
        {
            builder.ToTable("ProcurementKeySets");
            base.Configure(builder);
            builder.Property(x => x.Id).UseHiLo("ProcurementKeySetSequence");
            builder.HasIndex(p => new { p.BuyerId, p.FinancialPlaningCenterId, p.ProcurementId, p.PurchaseKeyId }).IsUnique();
        }
    }
}
