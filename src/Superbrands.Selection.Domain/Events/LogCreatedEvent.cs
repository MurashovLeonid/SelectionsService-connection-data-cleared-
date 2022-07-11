using System;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain.Abstractions;
using Superbrands.Selection.Domain.Logs;

namespace Superbrands.Selection.Domain.Events
{
    public class LogCreatedEvent: IDomainEvent
    {
        public LogEntry LogEntry { get; }

        public LogCreatedEvent(LogEntry logEntry)
        {
            LogEntry = logEntry ?? throw new ArgumentNullException(nameof(logEntry));
        }
    }
}