using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Superbrands.Selection.Domain.Logs;
using Superbrands.Selection.Infrastructure;
using Superbrands.Selection.Infrastructure.Abstractions;

namespace Superbrands.Selection.Application.Logs
{
    public class GetLogsBySelectionIdHandler : IRequestHandler<GetLogsBySelectionId, List<LogEntry>>
    {
        private readonly ISelectionRepository _repository;

        public GetLogsBySelectionIdHandler(ISelectionRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<LogEntry>> Handle(GetLogsBySelectionId request,
            CancellationToken cancellationToken)
        {
            var logs = await _repository.GetLogsBySelectionId(request.SelectionId, cancellationToken);
            return logs.Select(x => x.ToDomain()).ToList();
        }
    }
}