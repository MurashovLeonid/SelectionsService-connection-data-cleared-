using MediatR;
using Superbrands.Selection.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Superbrands.Selection.Domain.Events;

namespace Superbrands.Selection.Application.Procurement
{
    public class GetProcurementByIdQueryHandler : IRequestHandler<GetProcurementByIdQuery, Domain.Procurements.Procurement>
    {
        private readonly IProcurementRepository _repository;

        public GetProcurementByIdQueryHandler(IProcurementRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<Domain.Procurements.Procurement> Handle(GetProcurementByIdQuery request, CancellationToken cancellationToken)
        {
            var procurementDal = await _repository.GetById(request.Id, cancellationToken);
            return procurementDal?.ToDomain<ProcurementUpdatedEvent, Domain.Procurements.Procurement>();
        }
    }
}
