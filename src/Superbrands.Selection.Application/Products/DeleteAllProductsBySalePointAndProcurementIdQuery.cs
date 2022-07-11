using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Application.Products
{
    public class DeleteAllProductsBySalePointAndProcurementIdQuery : IRequest<Unit>
    {
        public int ProcurementId { get;  }
        public int SalePointId { get; }
        public DeleteAllProductsBySalePointAndProcurementIdQuery(int procurementId, int salePointId)
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
