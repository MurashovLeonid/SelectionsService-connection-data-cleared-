using MediatR;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Superbrands.Selection.Application.Templates.Handlers
{
    public class GetTemplateByIdQueryHandler : IRequestHandler<GetTemplateByIdQuery, NomenclatureTemplate>
    {
        private readonly IProcurementRepository _repository;

        public GetTemplateByIdQueryHandler(IProcurementRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<NomenclatureTemplate> Handle(GetTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            var nomenclatureTemplate = await _repository.GetNomenclatureTemplateById(request.TemplateId, cancellationToken);
            return nomenclatureTemplate.ToDomain();
        }
    }
}
