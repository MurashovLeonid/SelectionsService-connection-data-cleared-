using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Superbrands.Libs.RestClients.ContextualSearch;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Application.Procurement
{
    class SearchProcurementQueryHandler : IRequestHandler<SearchProcurementQuery, Libs.DDD.EfCore.PagedResult<ProcurementDalDto>>
    {
        private readonly IProcurementRepository _repository;
        private readonly IContextualsearchProcurementsearchClient _procurementSearchClient;

        public SearchProcurementQueryHandler(IProcurementRepository repository, IContextualsearchProcurementsearchClient procurementSearchClient)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _procurementSearchClient = procurementSearchClient ?? throw new ArgumentNullException(nameof(procurementSearchClient));
        }

        public async Task<Libs.DDD.EfCore.PagedResult<ProcurementDalDto>> Handle(SearchProcurementQuery request, CancellationToken cancellationToken)
        {
            var searchResult = await _procurementSearchClient.SearchProcurementsearchSearchAsync(request.PartnersIds, request.SeasonCapsulesIds,
                request.SalePointsIds, request.BuyersIds, request.Page, request.Size, cancellationToken);

            var procurementIds = searchResult.Documents.SelectMany(x => x.Value)
                .Select(x => x.GetEntityId().GetValueOrDefault()).ToList();


            return new Libs.DDD.EfCore.PagedResult<ProcurementDalDto>()
            {
                CurrentPage = request.Page.GetValueOrDefault(),
                PageSize = request.Size ?? 10,
                PageCount = (int) searchResult.DocumentsFound / (request.Size ?? 10),
                Results = await _repository.GetByIds(procurementIds, cancellationToken),
                RowCount = (int)searchResult.DocumentsFound
            };
        }
    }
}