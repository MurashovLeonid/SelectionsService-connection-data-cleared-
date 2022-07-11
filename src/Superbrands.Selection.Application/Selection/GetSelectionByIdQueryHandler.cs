using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Superbrands.Selection.Infrastructure;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Application.Selection
{
    public class GetSelectionByIdQueryHandler : IRequestHandler<GetSelectionByIdQuery, Domain.Selections.Selection>
    {
        private readonly ISelectionRepository _repository;

        public GetSelectionByIdQueryHandler(ISelectionRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Domain.Selections.Selection> Handle(GetSelectionByIdQuery request, CancellationToken cancellationToken)
        {
            var selection = await _repository.GetById(request.SelectionId, cancellationToken);
            return selection?.ToDomain();
        }
    }
}