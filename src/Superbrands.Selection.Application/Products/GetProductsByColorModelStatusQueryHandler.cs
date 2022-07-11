using MediatR;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.Helpers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Superbrands.Libs.RestClients.Pim;

namespace Superbrands.Selection.Application.Products
{
    public class GetProductsByColorModelStatusQueryHandler : IRequestHandler<GetProductsByColorModelStatusQuery, PagedResult_1OfProductData>
    {
        private readonly IPimBbproductsClient _pimClient;
        private IProductMetaRepository _productMetaRepository { get; set; }


        public GetProductsByColorModelStatusQueryHandler(IPimBbproductsClient pimClient, [NotNull] IProductMetaRepository productMetaRepository)
        {
            _pimClient = pimClient ?? throw new ArgumentNullException(nameof(pimClient));
            _productMetaRepository = productMetaRepository ?? throw new ArgumentNullException(nameof(productMetaRepository));
        }

        public async Task<PagedResult_1OfProductData> Handle(GetProductsByColorModelStatusQuery request, CancellationToken cancellationToken)
        {
            var colorModelMetas = await _productMetaRepository.GetProductsMetaByColorModelStatus(request.ColorModelStatus, cancellationToken);

            if (colorModelMetas.Any())
                return new PagedResult_1OfProductData();

            var vendorCodes = colorModelMetas.Select(p => p.ModelVendorCodeSbs).Cast<object>().ToArray();
            request.SearchProductsRequest.Filters.Add("ModelVendorCodeSbs", vendorCodes);
            var pimProducts = await _pimClient.SearchAsync(request.SearchProductsRequest, cancellationToken);
            SelectionHelper.FilterOutProductsNotInSelection(colorModelMetas.Select(x=>x.ToDomain()), pimProducts.Results);
            return pimProducts;
        }
    }
}
