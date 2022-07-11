using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Domain.Logs;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Infrastructure.Configurations
{
    public class LogConfiguration : EntityConfigurationBase<LogEntryDalDto>
    {
        public override void Configure(EntityTypeBuilder<LogEntryDalDto> builder)
        {
            base.Configure(builder);
            builder.ToTable("Logs");
            builder.Property(x => x.Id).UseHiLo("LogSeq");
            builder.Property(x => x.SelectionId).IsRequired();
            builder.Property(x => x.Message).IsRequired();

            builder.Metadata.FindProperty(nameof(LogEntry.Message))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}