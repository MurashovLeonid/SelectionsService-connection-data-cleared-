using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Superbrands.Selection.Domain.Events;
using Superbrands.Selection.Infrastructure;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Application.Logs
{
    public class WriteLogHandler : INotificationHandler<LogCreatedEvent>
    {
        private readonly ISelectionRepository _selectionRepository;

        public WriteLogHandler(ISelectionRepository selectionRepository)
        {
            _selectionRepository = selectionRepository ?? throw new ArgumentNullException(nameof(selectionRepository));
        }

        public Task Handle(LogCreatedEvent log, CancellationToken cancellationToken)
        {
            return _selectionRepository.AddLogsToSelection(new[] {LogEntryDalDto.FromDomain(log.LogEntry)}, cancellationToken);
        }
    }
}