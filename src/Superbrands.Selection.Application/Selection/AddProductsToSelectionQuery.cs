using System;
using System.Collections.Generic;
using MediatR;
using Superbrands.Libs.DDD.Abstractions;

namespace Superbrands.Selection.Application.Selection
{
    public class AddProductsToSelectionQuery : IRequest<Unit>
    {
        public IEnumerable<long> SalePointIds { get; }

        public long ProcurementId { get; }

        public List<ModelWithColors> Products { get; }

        public AddProductsToSelectionQuery(IEnumerable<long> salePointsIds, List<ModelWithColors> products, long procurementId)
        {
            SalePointIds = salePointsIds ?? throw new ArgumentNullException(nameof(salePointsIds));
            Products = products;
            if (procurementId < 0)
                throw new ArgumentNullException(nameof(salePointsIds));
            ProcurementId = procurementId;
        }
    }
}