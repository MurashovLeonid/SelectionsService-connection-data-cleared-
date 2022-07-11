using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;

namespace Superbrands.Selection.Application.Procurement
{
    public class GetProcurementsByIdsQuery : IRequest<List<Domain.Procurements.Procurement>>
    {
        public GetProcurementsByIdsQuery(IEnumerable<long> ids)
        {
            if (ids == null || !ids.Any())
                throw new ArgumentException(nameof(ids), "Ids cannot be empty");
            Ids = ids;
        }

        public IEnumerable<long> Ids { get; }
    }
}