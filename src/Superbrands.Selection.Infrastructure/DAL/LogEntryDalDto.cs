using Mapster;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Domain.Logs;

namespace Superbrands.Selection.Infrastructure.DAL
{
    public class LogEntryDalDto : AuditableEntity
    {
        public long SelectionId { get; internal set; }
        public string Message { get; internal set; }

        public LogEntry ToDomain()
        {
            return new(Message, SelectionId) { Id = Id };
        }

        public static LogEntryDalDto FromDomain(LogEntry logLogEntry)
        {
            return logLogEntry.Adapt<LogEntryDalDto>();
        }
    }
}
