using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Superbrands.Libs.DDD.EfCore.Abstractions;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Domain.Requests;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Infrastructure.Abstractions
{
    public interface IProductMetaRepository : IRepository<ColorModelMetaDalDto>
    {
        Task<List<ColorModelMetaDalDto>> GetProductsMetaByColorModelStatus(ColorModelStatus colorModelStatus, CancellationToken cancellationToken);
        Task DeleteProductsByIds(IEnumerable<long> ids, CancellationToken cancellationToken);
        Task<ColorModelMetaDalDto> ChangeSizeChartAndSizesCount(long colorModelMetaId, IEnumerable<SizeInfo> sizeInfos,
            int sizeChartCount, CancellationToken cancellationToken, int sizeChartId);
       Task<List<ColorModelMetaDalDto>> Get(List<string> modelVendoreCodesSbs, long? procurementId, long? selectionId,
           long? purchaseKeyId, long? salePointId, CancellationToken cancellationToken);
    }
}
