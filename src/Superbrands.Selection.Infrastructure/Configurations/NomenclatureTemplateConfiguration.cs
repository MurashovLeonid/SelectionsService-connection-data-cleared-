using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Infrastructure.DAL;
using System.Collections.Generic;
using Superbrands.Libs.DDD.EfCore.Extensions;

namespace Superbrands.Selection.Infrastructure.Configurations
{
    public class NomenclatureTemplateConfiguration : EntityConfigurationBase<NomenclatureTemplateDalDto>
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<NomenclatureTemplateDalDto> builder)
        {
            base.Configure(builder);
            builder.Property(b => b.Id).UseHiLo("NomenclatureTemplateSequence");
            builder.Property(x => x.EntityModificationInfo).IsJsonb().IsRequired();
            builder.Property(d => d.Parameters).HasColumnType("jsonb").HasConversion(
                        data => JsonConvert.SerializeObject(data),
                        str => JsonConvert.DeserializeObject<IEnumerable<int>>(str));
        }
    }
}
