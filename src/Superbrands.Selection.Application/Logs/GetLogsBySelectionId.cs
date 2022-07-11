using System;
using System.Collections.Generic;
using MediatR;
using Superbrands.Selection.Domain.Logs;

namespace Superbrands.Selection.Application.Logs
{
    public class GetLogsBySelectionId : IRequest<List<LogEntry>>
    {
        public GetLogsBySelectionId(long selectionId)
        {
            if (selectionId <= 0) throw new ArgumentOutOfRangeException(nameof(selectionId));
            SelectionId = selectionId;
        }

        public long SelectionId { get; }
    }
}