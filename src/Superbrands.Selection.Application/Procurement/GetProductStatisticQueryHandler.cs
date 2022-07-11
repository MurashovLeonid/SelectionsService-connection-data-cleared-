using MediatR;
using Superbrands.Selection.Domain.Selections;
using Superbrands.Selection.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Superbrands.Selection.Application.Procurement
{
    class GetProductStatisticQueryHandler : IRequestHandler<GetProductStatisticQuery, List<ColorModelMeta>>
    {
        private readonly IProcurementRepository _repository;

        public GetProductStatisticQueryHandler(IProcurementRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<ColorModelMeta>> Handle(GetProductStatisticQuery request, CancellationToken cancellationToken)
        {
            var colorModelMetasDalDto = await _repository.GetProductStatisticQuery(request.ProcurementsId, request.ModelVendoreCodes, cancellationToken);
            return colorModelMetasDalDto.Select(q => q.ToDomain()).ToList();
        }
    }
}
