using MediatR;

using Superbrands.Selection.Application.Requests;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Superbrands.Libs.RestClients.Pim;

namespace Superbrands.Selection.Application.Procurement
{
    public class GetProductIdsByGroupingParamsQuery : IRequest<PagedResult_1OfProductData>
    {
        public int ProcurementId { get; }
        public GroupingFilterParametersRequest GroupingFilterParametersRequest { get; }
        public Dictionary<dynamic, object> FilterParams { get; }
        public GetProductIdsByGroupingParamsQuery(int procurementId, GroupingFilterParametersRequest groupingFilterParametersRequest, Dictionary<dynamic, object> filterParams)
        {
            if (procurementId <= 0)
                throw new ArgumentNullException(nameof(procurementId));

            ProcurementId = procurementId;
            FilterParams = filterParams;
            GroupingFilterParametersRequest = groupingFilterParametersRequest ?? throw new ArgumentNullException(nameof(groupingFilterParametersRequest));
        }
    }
}
