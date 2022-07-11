using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Infrastructure.DAL;
using System.Collections.Generic;
using Superbrands.Libs.DDD.EfCore.Extensions;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.Infrastructure.Configurations
{
    public class ColorModelMetasConfiguration : EntityConfigurationBase<ColorModelMetaDalDto>
    {
        public override void Configure(EntityTypeBuilder<ColorModelMetaDalDto> builder)
        {
            base.Configure(builder);
            builder.Property(b => b.Id).UseHiLo("colorModelSeq");
            builder.HasIndex(x => new {x.ColorModelVendorCodeSbs, x.SelectionId, x.ModelVendorCodeSbs}).IsUnique();

            builder.Property((x => x.EntityModificationInfo)).IsJsonb().IsRequired();

            builder.Property(x => x.Sizes).HasConversion(data => JsonConvert.SerializeObject(data),
                data => JsonConvert.DeserializeObject<List<Size>>(data)).HasColumnType("jsonb");

            builder.OwnsOne(x => x.ColorModelGroupKeys).Property(x => x.ActivityId).HasColumnName("ActivityId");
            builder.OwnsOne(x => x.ColorModelGroupKeys).Property(x => x.ActivityTypeId).HasColumnName("ActivityTypeId");
            builder.OwnsOne(x => x.ColorModelGroupKeys).Property(x => x.AssortmentGroupId).HasColumnName("AssortmentGroupId");
            builder.OwnsOne(x => x.ColorModelGroupKeys).Property(x => x.BrandId).HasColumnName("BrandId");
            builder.OwnsOne(x => x.ColorModelGroupKeys).Property(x => x.PurchaseKeyId).HasColumnName("PurchaseKeyId");
            builder.OwnsOne(x => x.ColorModelGroupKeys).Property(x => x.SalePointId).HasColumnName("SalePointId");

            builder.HasOne(pks => pks.Selection).WithMany(pks => pks.ColorModelMetas)
                .HasForeignKey(pks => pks.SelectionId);
        }
    }
}