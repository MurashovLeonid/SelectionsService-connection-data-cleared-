using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Libs.DDD.EfCore.Extensions;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Infrastructure.Configurations
{
    public class ProcurementConfiguration : EntityConfigurationBase<ProcurementDalDto>
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<ProcurementDalDto> builder)
        {
            builder.ToTable("Procurements");
            base.Configure(builder);
            builder.Property(x => x.Id).UseHiLo("ProcurementSequence");
            builder.HasIndex(x => x.ExternalKeyFrom1C).IsUnique();
            builder.Property(d => d.SalePoints).IsJsonb();
            builder.Property(d => d.Brands).HasColumnType("jsonb").IsJsonb();
            builder.Property(x => x.CounterpartyConditions).IsJsonb();
            builder.Property(x => x.PartnerTeamMembersIds).IsJsonb();
            builder.Property(x => x.SbsTeamMembersIds).IsJsonb();
        }
    }
}
