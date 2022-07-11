using MediatR;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Superbrands.Selection.Application.Templates.Handlers
{
    public class GetAllTemplatesQueryHandler : IRequestHandler<GetAllTemplatesQuery, List<NomenclatureTemplate>>
    {
        private readonly IProcurementRepository _repository;

        public GetAllTemplatesQueryHandler(IProcurementRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<NomenclatureTemplate>> Handle(GetAllTemplatesQuery request, CancellationToken cancellationToken)
        {
            var nomenclatureTemplates = await _repository.GetAllNomenclatureTemplates(cancellationToken);
            return nomenclatureTemplates.Select(x => x.ToDomain()).ToList();
        }
    }
}
