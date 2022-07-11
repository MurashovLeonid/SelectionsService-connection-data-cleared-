using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Superbrands.Selection.Infrastructure.Abstractions;

namespace Superbrands.Selection.Application.Selection
{
    internal class GetSelectionsByProcurementIdQueryHandler : IRequestHandler<GetSelectionsByProcurementIdQuery, List<Domain.Selections.Selection>>
    {
        private readonly ISelectionRepository _selectionRepository;

        public GetSelectionsByProcurementIdQueryHandler([NotNull] ISelectionRepository selectionRepository)
        {
            _selectionRepository = selectionRepository ?? throw new ArgumentNullException(nameof(selectionRepository));
        }

        public async Task<List<Domain.Selections.Selection>> Handle(GetSelectionsByProcurementIdQuery request, CancellationToken cancellationToken)
        {
            var selectionsDalDtos = await _selectionRepository.GetByProcurementId(request.ProcurementId, cancellationToken);
            return selectionsDalDtos.Select(s => s.ToDomain()).ToList();
        }
    }
}