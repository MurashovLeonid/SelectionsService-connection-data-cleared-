using System;
using System.Collections.Generic;
using MediatR;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.Procurements;
using Superbrands.Selection.Infrastructure.RepositoryResponses;

namespace Superbrands.Selection.Application.Procurement
{
    public class GroupProcurementsQuery : IRequest<IEnumerable<ProcurementGroup>>
    {
        public BTSelectionsGroupResponse ResponseFromBt { get; }

        public GroupProcurementsQuery(BTSelectionsGroupResponse responseFromBt)
        {
            ResponseFromBt = responseFromBt ?? throw new ArgumentNullException(nameof(responseFromBt));
        }
    }
}
