using MediatR;
using Superbrands.Libs.RestClients.PIM;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Superbrands.Libs.RestClients.Pim;


namespace Superbrands.Selection.Application.Procurement
{
    internal class GetProductIdsByGroupingParamsQueryHandler : IRequestHandler<GetProductIdsByGroupingParamsQuery,
        PagedResult_1OfProductData>
    {
        private readonly IProcurementRepository _repository;
        private readonly IPimBbproductsClient _pimClient;

        public GetProductIdsByGroupingParamsQueryHandler(IProcurementRepository repository, IPimBbproductsClient pimClient)
        {
            _pimClient = pimClient ?? throw new ArgumentNullException(nameof(pimClient));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<PagedResult_1OfProductData> Handle(GetProductIdsByGroupingParamsQuery request,
            CancellationToken cancellationToken)
        {
            var products = await _repository.GetProductsByGroupingParameters(request.ProcurementId,
                request.GroupingFilterParametersRequest.GroupKeyType, request.GroupingFilterParametersRequest.GroupKeyId,
                cancellationToken);
            
            var vendorCodes = products.Select(cm => cm.ModelVendorCodeSbs).Cast<object>().ToArray();
            request.GroupingFilterParametersRequest.Filters.Add("ModelVendorCodeSbs", vendorCodes);
            
            var pimProducts =
                await _pimClient.SearchAsync(request.GroupingFilterParametersRequest, cancellationToken);

            foreach (var pimProduct in pimProducts.Results)
            {
                var sizesFromSelections = products
                    .Where(p => p.ModelVendorCodeSbs == pimProduct.ModelVendorCodeSbs)
                    .SelectMany(dto => dto.Sizes);

                foreach (var sizeFromPim in pimProduct.ColorLevel.SelectMany(cl => cl.RangeSizeLevel))
                    sizeFromPim.Count = sizesFromSelections.FirstOrDefault(sz => sz.Sku == sizeFromPim.Sku)?.Count;
            }

            return pimProducts;
        }
    }
}