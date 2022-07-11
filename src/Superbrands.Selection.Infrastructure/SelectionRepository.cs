using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;

#pragma warning disable 1998

namespace Superbrands.Selection.Infrastructure
{
    internal class SelectionRepository : RepositoryBase<SelectionDalDto>, ISelectionRepository
    {
        private readonly SelectionDbContext _context;

        public SelectionRepository(SelectionDbContext context, IHttpContextAccessor httpContextAccessor) : base(context,
            httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public override Task<SelectionDalDto> GetById(long id, CancellationToken cancellationToken)
        {
            return _context.Selections.AsNoTracking()
                .Include(x => x.ColorModelMetas)
                .ThenInclude(x => x.ColorModelGroupKeys)
                .Include(s=>s.Procurement)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public Task<List<SelectionDalDto>> GetSelectionsBySalePointIds(IEnumerable<long> salePointIds,
            CancellationToken cancellationToken)
        {
            return _context.Selections
                .Include(s => s.SelectionPurchaseSalePointKeys)
                .Include(s => s.Procurement)
                .Include(s => s.ColorModelMetas)
                .ThenInclude(x => x.ColorModelGroupKeys)
                .Where(s => s.SelectionPurchaseSalePointKeys
                    .Any(spd => salePointIds.Contains(spd.SalePointId))).ToListAsync(cancellationToken);
        }

        public async Task<List<SelectionDalDto>> GetSelectionsByIds(IEnumerable<long> selectionIds,
            CancellationToken cancellationToken)
        {
            var selections = await GetAll()
                .Where(s => selectionIds.Contains(s.Id))
                .Include(s=>s.Procurement)
                .Include(s => s.ColorModelMetas).ThenInclude(x => x.ColorModelGroupKeys)
                .ToListAsync(cancellationToken);
            return selections;
        }

        public async Task AddSelectionPurchaseSalePointKeys(IEnumerable<SelectionPurchaseSalePointKeyDalDto> dalDtoKeys)
        {
            _context.SelectionPurchaseSalePointKeys.AddRange(dalDtoKeys);
        }

        public async Task AddProductToSelection(ColorModelMetaDalDto productDb, CancellationToken cancellationToken)
        {
            await _context.ColorModelMetas.AddAsync(productDb, cancellationToken);
        }

        public async Task UpdateProduct(ColorModelMetaDalDto product, CancellationToken cancellationToken)
        {
            _context.ColorModelMetas.Update(product);
        }

        public Task<List<LogEntryDalDto>> GetLogsBySelectionId(long selectionId, CancellationToken cancellationToken)
        {
            return _context.Logs.Where(x => x.SelectionId == selectionId).AsNoTracking().ToListAsync(cancellationToken);
        }

        public Task AddLogsToSelection(IEnumerable<LogEntryDalDto> logs, CancellationToken cancellationToken)
        {
            if (logs == null) throw new ArgumentNullException(nameof(logs));
            if (logs.Any(x => x.SelectionId == 0))
                throw new ArgumentException(nameof(logs), "All logs should have selection Id");

            return _context.Logs.AddRangeAsync(logs, cancellationToken);
        }

        public Task<List<SelectionDalDto>> GetAvailableSelections(long userId, long partnerId, long? managerId,
            long[] buyersIds, CancellationToken cancellationToken)
        {
            var byersIdsStr = buyersIds.Select(x => x.ToString());
            var query = GetAll().Where(s => s.Procurement.PartnerId == partnerId);
            query = query.Where(s => s.EntityModificationInfo.Created.OperatorId == userId.ToString()
                                     || s.Status == SelectionStatus.Agreed
                                     || byersIdsStr.Contains(s.EntityModificationInfo.Created.OperatorId)
                                     || managerId.HasValue && s.EntityModificationInfo.Created.OperatorId ==
                                     managerId.ToString());

            return query.ToListAsync(cancellationToken);
        }

        public void RemoveProducts(List<ColorModelMetaDalDto> colorModelMetas)
        {
            _context.ColorModelMetas.RemoveRange(colorModelMetas);
        }

        public void Detach(SelectionDalDto selectionDalDto)
        {
            var entityEntry = _context.Entry(selectionDalDto);
            entityEntry.State = EntityState.Detached;
        }

        public Task<List<SelectionDalDto>> GetByProcurementId(long procurementId, CancellationToken cancellationToken)
        {
            return _context.Selections
                .Include(s => s.SelectionPurchaseSalePointKeys)
                .Include(s => s.Procurement)
                .Include(s => s.ColorModelMetas)
                .ThenInclude(x => x.ColorModelGroupKeys)
                .Where(s => s.ProcurementId == procurementId)
                .AsNoTracking().ToListAsync(cancellationToken);
        }

        public Task<List<SelectionDalDto>> GetSelectionsBySalePointIdAndPurchaseKeyIds(IEnumerable<long> salePointIds,
            IEnumerable<long> purchaseKeyIds,
            IEnumerable<long> seasonCapsuleIds,
            long procurementId,
            CancellationToken cancellationToken)
        {
            return _context.Selections.AsNoTracking()
                .Include(s => s.SelectionPurchaseSalePointKeys)
                .Include(s => s.Procurement)
                .Include(s => s.ColorModelMetas)
                .ThenInclude(x => x.ColorModelGroupKeys)
                .Where(s => s.SelectionPurchaseSalePointKeys.Any(spd =>
                                salePointIds.Contains(spd.SalePointId) && purchaseKeyIds.Contains(spd.PurchaseKeyId))
                            && seasonCapsuleIds.Contains(s.Procurement.SeasonId)
                            && s.ProcurementId == procurementId
                            ).ToListAsync(cancellationToken);
        }

        public Task<List<SelectionDalDto>> GetSelectionsBySalePointAndColorModelVendorCodes(IEnumerable<long> salePointIds,
            IEnumerable<string> colorModelVendorCodeSbs, CancellationToken cancellationToken)
        {
            return _context.Selections
                .Include(s => s.SelectionPurchaseSalePointKeys)
                .Include(s => s.Procurement)
                .Include(s => s.ColorModelMetas)
                .ThenInclude(x => x.ColorModelGroupKeys)
                .AsNoTracking()
                .Where(s => s.SelectionPurchaseSalePointKeys
                    .Any(spd => salePointIds.Contains(spd.SalePointId)))
                .Where(ss => ss.ColorModelMetas.Any(cm => colorModelVendorCodeSbs.Contains(cm.ColorModelVendorCodeSbs)))
                .ToListAsync(cancellationToken);
        }

        public Task<List<SelectionDalDto>> GetSelectionsByColorModelVendorCodes(IEnumerable<string> colorModelVendorCodeSbs,
            CancellationToken cancellationToken)
        {
            return _context.Selections
                .Include(s => s.SelectionPurchaseSalePointKeys)
                .Include(s => s.Procurement)
                .Include(s => s.ColorModelMetas)
                .ThenInclude(x => x.ColorModelGroupKeys)
                .AsNoTracking()
                .Where(s => s.ColorModelMetas
                    .Any(spd => colorModelVendorCodeSbs.Contains(spd.ColorModelVendorCodeSbs))).ToListAsync(cancellationToken);
        }
    }
}