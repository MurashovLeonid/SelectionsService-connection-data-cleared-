using System;
using System.Runtime.Serialization;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain.Abstractions;

namespace Superbrands.Selection.Domain.Logs
{
    [DataContract]
    public class LogEntry : EntityBase
    {
        protected LogEntry()
        {
        }

        public LogEntry(string message, long selectionId)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));
            if (selectionId < 0) throw new ArgumentOutOfRangeException(nameof(selectionId));

            Message = message;
            SelectionId = selectionId;
        }

        [DataMember]
        public long SelectionId { get; protected set; }
        
        [DataMember]
        public string Message { get; protected set; }
    }
}