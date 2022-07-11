using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Superbrands.Selection.Domain.Events;
using Superbrands.Selection.Infrastructure.Abstractions;

namespace Superbrands.Selection.Application.Procurement
{
    internal class GetProcurementsByIdsQueryHandler : IRequestHandler<GetProcurementsByIdsQuery, List<Domain.Procurements.Procurement>>
    {
        private readonly IProcurementRepository _repository;

        public GetProcurementsByIdsQueryHandler([NotNull] IProcurementRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<Domain.Procurements.Procurement>> Handle(GetProcurementsByIdsQuery request, CancellationToken cancellationToken)
        {
            var dtos = await _repository.GetProcurementsByIds(request.Ids, cancellationToken);
            return dtos.Select(x => x.ToDomain<ProcurementUpdatedEvent, Domain.Procurements.Procurement>()).ToList();
        }
    }
}