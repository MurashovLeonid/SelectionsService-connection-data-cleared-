using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Domain.Requests;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Infrastructure.DAL;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Domain;

namespace Superbrands.Selection.Infrastructure
{
    internal class ProductMetaRepository : RepositoryBase<ColorModelMetaDalDto>, IProductMetaRepository
    {
        private readonly SelectionDbContext _context;

        public ProductMetaRepository(SelectionDbContext context, IHttpContextAccessor httpContextAccessor) : base(context,
            httpContextAccessor)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task DeleteProductsByIds(IEnumerable<long> ids, CancellationToken cancellationToken)
        {
            var colorModels = await _context.ColorModelMetas.Where(cm => ids.Contains(cm.Id)).ToListAsync(cancellationToken);
            _context.ColorModelMetas.RemoveRange(colorModels);
        }

        public Task<List<ColorModelMetaDalDto>> Get(List<string> modelVendoreCodesSbs, long? procurementId,
            long? selectionId, long? purchaseKeyId, long? salePointId, CancellationToken cancellationToken)
        {
            var query = _context.ColorModelMetas.Where(c => modelVendoreCodesSbs.Contains(c.ModelVendorCodeSbs));
            if (procurementId.HasValue)
                query = query.Where(c => c.Selection.ProcurementId == procurementId);
            if (purchaseKeyId.HasValue)
                query = query.Where(c => c.ColorModelGroupKeys.PurchaseKeyId == purchaseKeyId);
            if (salePointId.HasValue)
                query = query.Where(c => c.ColorModelGroupKeys.SalePointId == salePointId);
            if (selectionId.HasValue)
                query = query.Where(c => c.SelectionId == selectionId);

            return query.ToListAsync(cancellationToken);
        }

        public Task<List<ColorModelMetaDalDto>> GetProductsMetaByColorModelStatus(ColorModelStatus colorModelStatus,
            CancellationToken cancellationToken)
        {
            return _context.ColorModelMetas.Where(p => p.ColorModelStatus == colorModelStatus).ToListAsync(cancellationToken);
        }

        public async Task<ColorModelMetaDalDto> ChangeSizeChartAndSizesCount(long colorModelMetaId,
            IEnumerable<SizeInfo> sizeInfos, int sizeChartCount, CancellationToken cancellationToken, int sizeChartId)
        {
            var colorModelMeta =
                await _context.ColorModelMetas.FirstOrDefaultAsync(p => p.Id == colorModelMetaId, cancellationToken);
            if (colorModelMeta == null)
                throw new ArgumentNullException($"colorModelMeta with Id {colorModelMetaId} is not found");

            colorModelMeta.SizeChartCount = sizeChartCount;
            colorModelMeta.SizeChartId = sizeChartId;
            foreach (var sizeInf in sizeInfos)
            {
                var size = colorModelMeta.Sizes.FirstOrDefault(sz => sz.Sku == sizeInf.Sku);
                if (size == null)
                {
                    size = new Size(sizeInf.Sku);
                    colorModelMeta.Sizes.Add(size);
                }

                size.Count = sizeInf.Count;
                size.Bwp = sizeInf.Bwp;
            }

           // _context.ColorModelMetas.Update(colorModelMeta);
            
            return colorModelMeta;
        }
    }
}