using System.Collections.Generic;
using MediatR;

namespace Superbrands.Selection.Application.Selection
{
    public class GetSelectionByIdQuery : IRequest<Domain.Selections.Selection>
    {
        public GetSelectionByIdQuery(long selectionId)
        {
            SelectionId = selectionId;
        }

        public long SelectionId { get; }
    }
}