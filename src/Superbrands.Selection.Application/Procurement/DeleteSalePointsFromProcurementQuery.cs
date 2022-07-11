using MediatR;
using Superbrands.Libs.DDD.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Application.Procurement
{
    public class DeleteSalePointsFromProcurementQuery : IRequest<List<Domain.Procurements.Procurement>>
    {
        public List<long> SalePointIds { get; set; }
       
        public DeleteSalePointsFromProcurementQuery(List<long> salePointIds)
        {
            SalePointIds = salePointIds;
        }
    }
}
