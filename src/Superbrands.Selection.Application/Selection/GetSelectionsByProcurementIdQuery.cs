using System.Collections.Generic;
using MediatR;

namespace Superbrands.Selection.Application.Selection
{
    public class GetSelectionsByProcurementIdQuery : IRequest<List<Domain.Selections.Selection>>
    {
        public GetSelectionsByProcurementIdQuery(long procurementId)
        {
            ProcurementId = procurementId;
        }

        public long ProcurementId { get; }
    }
}