using MediatR;
using Superbrands.Libs.RestClients.PIM;
using Superbrands.Selection.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Superbrands.Libs.RestClients.Pim;
using Superbrands.Selection.Application.Requests;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.Selections;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Application.Selection
{
    public class 
        ChangeSizesWithSizeChartQueryHandler : IRequestHandler<ChangeSizesWithSizeChartQuery, Unit>
    {
        private readonly IProductMetaRepository _productMetaRepository;
        private readonly ISelectionRepository _selectionRepository;
        private readonly IPimBbproductsClient _pimClient;

        public ChangeSizesWithSizeChartQueryHandler(IPimBbproductsClient pimClient, IProductMetaRepository productMetaRepository, ISelectionRepository selectionRepository)
        {
            _pimClient = pimClient ?? throw new ArgumentNullException(nameof(pimClient));
            _productMetaRepository = productMetaRepository ?? throw new ArgumentNullException(nameof(productMetaRepository));
            _selectionRepository = selectionRepository ?? throw new ArgumentNullException(nameof(selectionRepository));
        }

        public async Task<Unit> Handle(ChangeSizesWithSizeChartQuery request, CancellationToken cancellationToken)
        {
            var selections = request.SalePointIds.Any() ?
            await _selectionRepository.GetSelectionsBySalePointAndColorModelVendorCodes(request.SalePointIds, new List<string> { request.ColorModelVendorCodeSbs }, cancellationToken) :
            await _selectionRepository.GetSelectionsByColorModelVendorCodes(new List<string>() { request.ColorModelVendorCodeSbs }, cancellationToken);

            if (!selections.Any())
                throw new Exception(request.SalePointIds.Any() ? $"Данной цветомодели {request.ColorModelVendorCodeSbs}, нет в отборках на входной массив точек продаж" : "Данной цветомодели {request.ColorModelId}, нет в отборках ");

            var colorModels = selections.SelectMany(s => s.ColorModelMetas);
            var fitSizes = colorModels.SelectMany(cm => cm.Sizes).Where(sz => request.SizesSkuAndCount.Any(ss => ss.SizeSku.Contains(sz.Sku))).Select(s=>s.Sku).Distinct();
            var absentSizes = request.SizesSkuAndCount.Where(s => !fitSizes.Contains(s.SizeSku)).Select(s=>s.SizeSku);

            var sizesFromPim = await GetSizesFromPim(fitSizes, cancellationToken);
            await RewriteColorModels(colorModels.Select(x=>x.ToDomain()), request.SizesSkuAndCount, fitSizes, sizesFromPim, request.SizeChartId, request.SizeChartCount, cancellationToken);
            return Unit.Value;
        }

        private async Task RewriteColorModels(IEnumerable<ColorModelMeta> colorModelMetas, IEnumerable<SizeSkuAndCount> sizesSkuAndCount, IEnumerable<string> fitSizes, IEnumerable<RangeSizeLevel> sizesFromPim,
            int sizeChartId, int sizeChartCount, CancellationToken cancellationToken)
        {
            foreach (var colorModelMeta in colorModelMetas)
            {
                colorModelMeta.SizeChartId = sizeChartId;
                colorModelMeta.SizeChartCount = sizeChartCount;
                colorModelMeta.Sizes = colorModelMeta.Sizes.Where(sz => fitSizes.Contains(sz.Sku)).ToList();
                colorModelMeta.ChangeColorModelStatus(colorModelMeta.ColorModelStatus ^ Domain.Enums.ColorModelStatus.New);

                foreach (var size in colorModelMeta.Sizes)
                    size.Count = sizesSkuAndCount.FirstOrDefault(s => s.SizeSku == size.Sku).SizeCount;

                foreach (var size in sizesFromPim)
                {
                    var newSize = new Size(size.Sku, sizesSkuAndCount.FirstOrDefault(s => s.SizeSku == size.Sku).SizeCount, (int)size.Bwp.GetValueOrDefault(), 1);
                    colorModelMeta.Sizes.Add(newSize);
                }
                var dal = ColorModelMetaDalDto.FromDomain(colorModelMeta);
                await _productMetaRepository.Update(dal, cancellationToken);
            }
            await _productMetaRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<IEnumerable<RangeSizeLevel>> GetSizesFromPim(IEnumerable<string> absentSizes, CancellationToken cancellationToken)
        {
            if (!absentSizes.Any())
                return new List<RangeSizeLevel>();

            var filterParams = new Dictionary<string, ICollection<object>> {{"ProductsSkus", absentSizes.ToArray()}};
            var searchParams = new SearchProductsRequest() { Filters = filterParams };

            var pimProducts = await _pimClient.SearchAsync(searchParams, cancellationToken);
            var sizesFromPim = pimProducts.Results.SelectMany(p => p.ColorLevel).SelectMany(cl => cl.RangeSizeLevel).Where(rs => absentSizes.Contains(rs.Sku));

            if (!sizesFromPim.Any() || sizesFromPim.Count() != absentSizes.Count())
                throw new Exception("Пришедших в запросе Sku размеров нет в пим.");

            return sizesFromPim.ToList();
        }
    }
}
