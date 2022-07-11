using MediatR;


using Superbrands.Selection.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Libs.RestClients.Pim;

namespace Superbrands.Selection.Application.Products
{
    public class GetProductsByColorModelStatusQuery : IRequest<PagedResult_1OfProductData>
    {
        public ColorModelStatus ColorModelStatus { get; }
        public SearchProductsRequest SearchProductsRequest { get; }

        public GetProductsByColorModelStatusQuery(ColorModelStatus colorModelStatus, SearchProductsRequest SearchProductsRequest)
        {
            ColorModelStatus = colorModelStatus;
            SearchProductsRequest = SearchProductsRequest ?? throw new ArgumentNullException(nameof(SearchProductsRequest));
        }
    }
}
