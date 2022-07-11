using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Superbrands.Libs.DDD.EfCore.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Infrastructure.Abstractions
{
    public interface ISelectionRepository : IRepository<SelectionDalDto>
    {
        Task AddSelectionPurchaseSalePointKeys(IEnumerable<SelectionPurchaseSalePointKeyDalDto> dalDtoKeys);
       Task<List<SelectionDalDto>> GetSelectionsByColorModelVendorCodes(IEnumerable<string> colorModelVendorCodes, CancellationToken cancellationToken);
        Task<List<SelectionDalDto>> GetSelectionsBySalePointAndColorModelVendorCodes(IEnumerable<long> salePointIds, IEnumerable<string> colorModelVendorCodes, CancellationToken cancellationToken);
        public Task<List<SelectionDalDto>> GetSelectionsBySalePointIdAndPurchaseKeyIds(IEnumerable<long> salePointIds, 
        IEnumerable<long> purchaseKeyIds,  IEnumerable<long> seasonCapsuleIds, long procurementId, CancellationToken cancellationToken);
        Task<List<SelectionDalDto>> GetSelectionsBySalePointIds(IEnumerable<long> salePointIds, CancellationToken cancellationToken);
       Task<List<SelectionDalDto>> GetSelectionsByIds(IEnumerable<long> selectionIds, CancellationToken cancellationToken);
        Task AddProductToSelection(ColorModelMetaDalDto productDb, CancellationToken cancellationToken);
        Task UpdateProduct(ColorModelMetaDalDto product, CancellationToken cancellationToken);
        Task<List<LogEntryDalDto>> GetLogsBySelectionId(long selectionId, CancellationToken cancellationToken);
        Task AddLogsToSelection(IEnumerable<LogEntryDalDto> logs, CancellationToken cancellationToken);

        /// <summary>
        /// Получает отборки доступные пользователю
        /// </summary>
        /// <param name="userId">Пользователь который запрашивает отборки</param>
        /// <param name="partnerId">Партнер пользователя</param>
        /// <param name="managerId">МРП пользолователя(если пользователь сам МРП — null)</param>
        /// <param name="buyersIds">байеры, если пользователь МРП</param>
        Task<List<SelectionDalDto>> GetAvailableSelections(long userId, long partnerId, long? managerId, long[] buyersIds, CancellationToken cancellationToken);

        void RemoveProducts(List<ColorModelMetaDalDto> colorModelMetas);
        void Detach(SelectionDalDto selectionDalDto);
        Task<List<SelectionDalDto>> GetByProcurementId(long procurementId, CancellationToken cancellationToken);
    }
}