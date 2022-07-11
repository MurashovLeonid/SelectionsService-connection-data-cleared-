using System.Collections.Generic;
using MediatR;

namespace Superbrands.Selection.Application.Selection
{
    public class GetSelectionsByIdsQuery : IRequest<List<Domain.Selections.Selection>>
    {
        public GetSelectionsByIdsQuery(List<long> ids)
        {
            Ids = ids;
        }

        public List<long> Ids { get; }
    }
}