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
    internal class GetSelectionsByIdsQueryHandler : IRequestHandler<GetSelectionsByIdsQuery, List<Domain.Selections.Selection>>
    {
        private readonly ISelectionRepository _selectionRepository;

        public GetSelectionsByIdsQueryHandler([NotNull] ISelectionRepository selectionRepository)
        {
            _selectionRepository = selectionRepository ?? throw new ArgumentNullException(nameof(selectionRepository));
        }

        public async Task<List<Domain.Selections.Selection>> Handle(GetSelectionsByIdsQuery request, CancellationToken cancellationToken)
        {
            var selections = await _selectionRepository.GetSelectionsByIds(request.Ids, cancellationToken);
            return selections.Select(s => s.ToDomain()).ToList();
        }
    }
}