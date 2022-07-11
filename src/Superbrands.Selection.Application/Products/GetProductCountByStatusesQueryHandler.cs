using MediatR;
using Superbrands.Selection.Application.Responses;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Superbrands.Libs.RestClients.Pim;

namespace Superbrands.Selection.Application.Products
{
    public class GetProductCountByStatusesQueryHandler : IRequestHandler<GetProductCountByStatusesQuery, IEnumerable<ProductStatusCount>>
    {
        private readonly IPimBbproductsClient _pimClient;
        private readonly IProcurementRepository _repository;

        public GetProductCountByStatusesQueryHandler(IProcurementRepository repository, IPimBbproductsClient pimClient)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _pimClient = pimClient ?? throw new ArgumentNullException(nameof(pimClient));
        }

        public async Task<IEnumerable<ProductStatusCount>> Handle(GetProductCountByStatusesQuery request, CancellationToken cancellationToken)
        {
            var products = await _repository.GetProductsByGroupingParameters(request.GroupingFilterParametersRequest.GroupKeyType, request.GroupingFilterParametersRequest.GroupKeyId, cancellationToken);
            var statusTaskDict = products.GroupBy(x => x.ColorModelStatus).ToDictionary(x => x.Key, x =>  SendRequestsForCount(request.GroupingFilterParametersRequest, x.Select(z => z.ModelVendorCodeSbs).Cast<object>().ToArray(), cancellationToken));
            var pairs = await Task.WhenAll(statusTaskDict.Select(async pair => new KeyValuePair<int, int>((int)pair.Key, await pair.Value)));
            var pairsDict = pairs.ToDictionary(p => p.Key);

            var result = new List<ProductStatusCount>
            {
                    new ProductStatusCount(ColorModelStatus.Archive, pairsDict[(int)ColorModelStatus.Archive].Value),
                    new ProductStatusCount(ColorModelStatus.Canceled, pairsDict[(int)ColorModelStatus.Canceled].Value),
                    new ProductStatusCount(ColorModelStatus.New, pairsDict[(int)ColorModelStatus.New].Value),
                    new ProductStatusCount(ColorModelStatus.PriceChanged, pairsDict[(int)ColorModelStatus.PriceChanged].Value),
                    new ProductStatusCount(ColorModelStatus.QuantityChanged, pairsDict[(int)ColorModelStatus.QuantityChanged].Value)
            };

            return result;
        }

        private async Task<int> SendRequestsForCount(SearchProductsRequest searchProductsRequest, object[] modelVendorCodes, CancellationToken cancellationToken)
        {
            searchProductsRequest.Filters.Add("ModelVendorCodeSbs", modelVendorCodes);
            searchProductsRequest.PageSize = 0;
            var pimProducts = await _pimClient.SearchAsync(searchProductsRequest, cancellationToken);
            searchProductsRequest.Filters.Remove("ModelVendorCodeSbs");
            return pimProducts.RowCount;
        }
    }
}
