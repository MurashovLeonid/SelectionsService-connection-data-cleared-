using MediatR;
using Superbrands.Selection.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Superbrands.Selection.Application.Products
{
    public class DeleteAllProductsBySalePointAndProcurementIdQueryHandler : IRequestHandler<DeleteAllProductsBySalePointAndProcurementIdQuery, Unit>
    {
        private readonly IProcurementRepository _procurementRepository;
        private readonly IProductMetaRepository _productMetaRepository;

        public DeleteAllProductsBySalePointAndProcurementIdQueryHandler(IProcurementRepository repository, IProductMetaRepository productMetaRepository)
        {
            _procurementRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _productMetaRepository = productMetaRepository ?? throw new ArgumentNullException(nameof(productMetaRepository));
        }
        public async Task<Unit> Handle(DeleteAllProductsBySalePointAndProcurementIdQuery request, CancellationToken cancellationToken)
        {
            var colorModelMetas = await _procurementRepository.GetProductsByProcurementSalePointIds(request.ProcurementId, request.SalePointId, cancellationToken);
            var colorModelMetasId = colorModelMetas.Select(s=> s.Id) ;
            await _productMetaRepository.DeleteProductsByIds(colorModelMetasId, cancellationToken);
            return Unit.Value;
        }
    }
}
