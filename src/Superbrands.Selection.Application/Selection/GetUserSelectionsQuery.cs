using System.Collections.Generic;
using MediatR;

namespace Superbrands.Selection.Application.Selection
{
    public class GetUserSelectionsQuery : IRequest<IEnumerable<Domain.Selections.Selection>>
    {
        public long UserId { get; }

        public GetUserSelectionsQuery(long userId)
        {
            UserId = userId;
        }
    }
}