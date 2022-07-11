using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using Superbrands.Selection.Application.Responses;

namespace Superbrands.Selection.Application.Products
{
    public class GetProductsBySBSQuery : IRequest<ICollection<ProductData>>
    {
        public GetProductsBySBSQuery(List<string> modelVendoreCodesSbs, long? selectionId, long? procurementId, long? salePointId, long? purchaseKeyId)
        {
            if (modelVendoreCodesSbs == null || !modelVendoreCodesSbs.Any())
                throw new ArgumentException("modelVendoreCodesSbs cannot be empty");
            ModelVendoreCodesSbs = modelVendoreCodesSbs;
            SelectionId = selectionId;
            ProcurementId = procurementId;
            SalePointId = salePointId;
            PurchaseKeyId = purchaseKeyId;
        }

        public List<string> ModelVendoreCodesSbs { get;  }
        public long? SelectionId { get;  }
        public long? ProcurementId { get; }
        public long? SalePointId { get; }
        public long? PurchaseKeyId { get;  }
    }
}