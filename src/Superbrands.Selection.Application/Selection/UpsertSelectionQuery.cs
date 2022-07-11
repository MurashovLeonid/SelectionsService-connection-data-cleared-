using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Superbrands.Selection.Infrastructure;
using Superbrands.Selection.Infrastructure.Abstractions;

namespace Superbrands.Selection.Application.Selection
{
    public class UpsertSelectionQuery : IRequest<Domain.Selections.Selection>
    {
        public Domain.Selections.Selection Selection { get; }

        public UpsertSelectionQuery(Domain.Selections.Selection selection)
        {
            Selection = selection ?? throw new ArgumentNullException(nameof(selection));
        }
    }
}