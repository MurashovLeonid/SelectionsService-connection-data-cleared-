using MediatR;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Superbrands.Selection.Application.Templates.Handlers
{
    public class CreateTemplateQueryHandler : IRequestHandler<CreateTemplateQuery, NomenclatureTemplate>
    {
        private readonly IProcurementRepository _repository;

        public CreateTemplateQueryHandler(IProcurementRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<NomenclatureTemplate> Handle(CreateTemplateQuery request, CancellationToken cancellationToken)
        {
            var nomenclatureTemplate = await  _repository.CreateNomenclatureTemplate(NomenclatureTemplateDalDto.FromDomain(request.NomenclatureTemplate), cancellationToken);
            return nomenclatureTemplate.ToDomain();
        }
    }
}
