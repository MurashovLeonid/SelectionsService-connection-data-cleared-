using MediatR;

using Superbrands.Selection.Application.Requests;
using Superbrands.Selection.Application.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using Superbrands.Libs.RestClients.Pim;

namespace Superbrands.Selection.Application.Products
{
    public class GetProductCountByStatusesQuery : IRequest<IEnumerable<ProductStatusCount>>
    {
        public GroupingFilterParametersRequest GroupingFilterParametersRequest { get; }

        public GetProductCountByStatusesQuery(GroupingFilterParametersRequest groupingFilterParametersRequest)
        {
            GroupingFilterParametersRequest = groupingFilterParametersRequest ?? throw new ArgumentNullException(nameof(groupingFilterParametersRequest));
        }
    }
}
