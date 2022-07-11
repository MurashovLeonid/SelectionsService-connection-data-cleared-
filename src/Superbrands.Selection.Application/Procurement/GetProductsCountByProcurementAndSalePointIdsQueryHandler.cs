using MediatR;
using Superbrands.Selection.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Superbrands.Selection.Application.Procurement
{
    public class GetProductsCountByProcurementAndSalePointIdsQueryHandler : IRequestHandler<GetProductsCountByProcurementAndSalePointIdsQuery, int>
    {
        private readonly IProcurementRepository _repository;

        public GetProductsCountByProcurementAndSalePointIdsQueryHandler(IProcurementRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<int> Handle(GetProductsCountByProcurementAndSalePointIdsQuery request, CancellationToken cancellationToken)
        {
            var colorModels = await _repository.GetProductsByProcurementSalePointIds(request.ProcurementId, request.SalePointId, cancellationToken);
            var productCount = colorModels.Where(cm => cm.ColorModelGroupKeys?.SalePointId == request.SalePointId).Sum(cmm => cmm.Sizes.Count);
            return productCount;
        }
    }
}
