using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Infrastructure.Configurations
{
    public class SelectionConfiguration : EntityConfigurationBase<SelectionDalDto>
    {
        public override void Configure(EntityTypeBuilder<SelectionDalDto> builder)
        {
            base.Configure(builder);
            builder.ToTable("Selection");
            builder.Property(b => b.Id).UseHiLo("selectionSeq");

            builder.Property(x => x.Status).HasDefaultValue(SelectionStatus.InProgress).IsRequired();

            builder.HasMany(x => x.Logs).WithOne().HasForeignKey(x => x.SelectionId);
            builder.HasOne(pks => pks.Procurement).WithMany(pks => pks.Selections)
                .HasForeignKey(pks => pks.ProcurementId);
        }
    }
}