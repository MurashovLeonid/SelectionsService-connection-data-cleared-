using Superbrands.Selection.Domain.Logs;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class LogEntryDto
    {
        public long Id { get; set; }
        public long SelectionId { get;  set; }
        public string Message { get;  set; }

        public LogEntryDto(LogEntry logEntry)
        {
            Id = logEntry.Id;
            SelectionId = logEntry.SelectionId;
            Message = logEntry.Message;
        }
    }
}
