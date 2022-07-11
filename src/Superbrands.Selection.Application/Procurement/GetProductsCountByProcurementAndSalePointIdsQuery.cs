using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Application.Procurement
{
    public class GetProductsCountByProcurementAndSalePointIdsQuery : IRequest<int>
    {
        public long ProcurementId { get; }
        public long SalePointId { get; }
        public GetProductsCountByProcurementAndSalePointIdsQuery(long procurementId, long salePointId)
        {
            if (procurementId <= 0)
                throw new ArgumentException(nameof(procurementId));

            if (salePointId <= 0)
                throw new ArgumentException(nameof(salePointId));

            ProcurementId = procurementId;
            SalePointId = salePointId;
        }
    }
}
