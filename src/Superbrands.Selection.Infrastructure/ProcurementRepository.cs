using Microsoft.EntityFrameworkCore;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.RepositoryResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Infrastructure
{
    internal class ProcurementRepository : RepositoryBase<ProcurementDalDto>, IProcurementRepository
    {
        private readonly SelectionDbContext _context;

        public ProcurementRepository(SelectionDbContext context, IHttpContextAccessor httpContextAccessor) : base(context,
            httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public override Task<ProcurementDalDto> GetById(long id, CancellationToken cancellationToken)
        {
            return _context.Procurements.AsNoTracking()
                .Include(x => x.Selections)
                .ThenInclude(x => x.SelectionPurchaseSalePointKeys)
                .Include(x => x.Selections)
                .ThenInclude(x => x.ColorModelMetas)
                .Include(x => x.ProcurementKeySets)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public Task<List<ColorModelMetaDalDto>> GetProductsByProcurementSalePointIds(long procurementId, long salePointId,
            CancellationToken cancellationToken)
        {
            return _context.Selections
                .Include(s => s.SelectionPurchaseSalePointKeys)
                .Include(s => s.ColorModelMetas)
                .Where(p => p.ProcurementId == procurementId &&
                            p.SelectionPurchaseSalePointKeys.Any(sp => sp.SalePointId == salePointId))
                .SelectMany(s => s.ColorModelMetas).ToListAsync(cancellationToken);
        }
        public Task<List<ProcurementDalDto>> GetBySeasonCapsuleIds(List<long> seasonCapsuleIds,
            CancellationToken cancellationToken)
        {
            return _context.Procurements
                    .Where(x => seasonCapsuleIds.Contains(x.SeasonId))
                    .ToListAsync(cancellationToken);
        }


        public async Task<IEnumerable<ColorModelMetaDalDto>> GetProductsByGroupingParameters(int procurementId,
            GroupKeyType groupKeyType, int groupKeyId, CancellationToken cancellationToken)
        {
            var procurement = await _context.Procurements
                .Include(p => p.Selections)
                .ThenInclude(p => p.ColorModelMetas)
                .FirstOrDefaultAsync(pr => pr.Id == procurementId, cancellationToken);

            if (procurement != null)
            {
                var models = procurement.Selections
                    .SelectMany(s => s.ColorModelMetas
                        .Where(cm => (groupKeyType == GroupKeyType.AssortmentGroupId &&
                                      cm.ColorModelGroupKeys.AssortmentGroupId == groupKeyId)
                                     || (groupKeyType == GroupKeyType.ActivityId &&
                                         cm.ColorModelGroupKeys.ActivityId == groupKeyId)
                                     || (groupKeyType == GroupKeyType.ActivityTypeId &&
                                         cm.ColorModelGroupKeys.ActivityTypeId == groupKeyId)
                                     || (groupKeyType == GroupKeyType.BrandId &&
                                         cm.ColorModelGroupKeys.BrandId == groupKeyId)
                                     || (groupKeyType == GroupKeyType.PurchaseKeyId &&
                                         cm.ColorModelGroupKeys.PurchaseKeyId == groupKeyId)
                                     || (groupKeyType == GroupKeyType.SalePointId &&
                                         cm.ColorModelGroupKeys.SalePointId == groupKeyId))
                    );
                return models;
            }

            return new List<ColorModelMetaDalDto>();
        }


        public void Update(ProcurementDalDto procurementDalDto)
        {
            _context.Update(procurementDalDto);
        }

        public async Task<IEnumerable<ColorModelMetaDalDto>> GetProductsByGroupingParameters(GroupKeyType groupKeyType,
            int groupKeyId,
            CancellationToken cancellationToken)
        {
            var colorModels = await _context.ColorModelMetas.Where(cm => (groupKeyType == GroupKeyType.AssortmentGroupId &&
                                                                          cm.ColorModelGroupKeys.AssortmentGroupId == groupKeyId)
                                                                         || groupKeyType == GroupKeyType.ActivityId &&
                                                                         cm.ColorModelGroupKeys.ActivityId == groupKeyId
                                                                         || groupKeyType == GroupKeyType.ActivityTypeId &&
                                                                         cm.ColorModelGroupKeys.ActivityTypeId == groupKeyId
                                                                         || groupKeyType == GroupKeyType.BrandId &&
                                                                         cm.ColorModelGroupKeys.BrandId == groupKeyId
                                                                         || groupKeyType == GroupKeyType.PurchaseKeyId &&
                                                                         cm.ColorModelGroupKeys.PurchaseKeyId == groupKeyId
                                                                         || groupKeyType == GroupKeyType.SalePointId &&
                                                                         cm.ColorModelGroupKeys.SalePointId == groupKeyId)
                .ToListAsync(cancellationToken);

            return colorModels;
        }


        public IEnumerable<SelectionDalDto> GetFilteredSelections(long procurementId, BTSelectionsGroupResponse responseFromBt,
            CancellationToken cancellationToken)
        {
            var salePointIds = responseFromBt.PurchaseKeyDepartment.SelectMany(c => new List<long> {c.SalePointId})
                .ToList();
            var purchaseKeyIds = responseFromBt.PurchaseKeyDepartment.SelectMany(c => new List<long> {c.PurchaseKeyId})
                .ToList();

            var procurement = _context.Procurements.Include(p => p.Selections)
                .ThenInclude(s => s.ColorModelMetas)
                .ThenInclude(cm => cm.ColorModelGroupKeys)
                .FirstOrDefault(p => p.Id == procurementId);

            var selections = procurement.Selections
                .Where(s => s.ColorModelMetas.Any(cm =>
                    salePointIds.Contains(cm.ColorModelGroupKeys.SalePointId) &&
                    purchaseKeyIds.Contains(cm.ColorModelGroupKeys.PurchaseKeyId))).ToList();

            return selections;
        }

        public IEnumerable<ProcurementDalDto> GetFilteredProcurements(BTSelectionsGroupResponse responseFromBt,
            CancellationToken cancellationToken)
        {
            var salePointIds = responseFromBt.PurchaseKeyDepartment.Select(c => c.SalePointId);
            var purchaseKeyIds = responseFromBt.PurchaseKeyDepartment.Select(c => c.PurchaseKeyId);

            var filteredProcurements = _context.Procurements
                .Include(p => p.Selections)
                .ThenInclude(s => s.ColorModelMetas)
                .ThenInclude(cm => cm.ColorModelGroupKeys)
                .Where(p => p.Selections.Any(s => s.ColorModelMetas.Any(cm =>
                    salePointIds.Contains(cm.ColorModelGroupKeys.SalePointId) &&
                    purchaseKeyIds.Contains(cm.ColorModelGroupKeys.PurchaseKeyId)))).ToList();

            return filteredProcurements;
        }

        public async Task<NomenclatureTemplateDalDto> CreateNomenclatureTemplate(NomenclatureTemplateDalDto nomenclatureTemplate,
            CancellationToken cancellationToken)
        {
            var createdEntity = await _context.NomenclatureTemplates.AddAsync(nomenclatureTemplate, cancellationToken);
            return createdEntity.Entity;
        }

        public Task<List<NomenclatureTemplateDalDto>> GetAllNomenclatureTemplates(CancellationToken cancellationToken)
        {
            return _context.NomenclatureTemplates.ToListAsync(cancellationToken);
        }

        public Task<NomenclatureTemplateDalDto> GetNomenclatureTemplateById(int templateId, CancellationToken cancellationToken)
        {
            return _context.NomenclatureTemplates.FirstOrDefaultAsync(nt => nt.Id == templateId, cancellationToken);
        }

        public Task<List<ProcurementDalDto>> GetProcurementsByIds(IEnumerable<long> ids, CancellationToken cancellationToken)
        {
            return _context.Procurements.AsNoTracking().Include(x => x.Selections)
                .ThenInclude(x => x.SelectionPurchaseSalePointKeys)
                .Include(x => x.Selections)
                .ThenInclude(x => x.ColorModelMetas).Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken);
        }
        public Task<List<ProcurementDalDto>> GetProcurementsBySalePointAttributes(long businessSegmentId, List<long> seasonIds, long partnerId, CancellationToken cancellationToken)
        {
            return _context.Procurements
                .Where(x=> x.PartnerId == partnerId && x.SegmentId == businessSegmentId &&  (seasonIds !=null && seasonIds.Contains(x.SeasonId)))?
                .ToListAsync(cancellationToken) ?? null;
        }


        public Task<List<ProcurementDalDto>> GetByIds(List<long> ids, CancellationToken cancellationToken)
        {
            return _context.Procurements.Where(x => ids.Contains(x.Id)).AsNoTracking().ToListAsync(cancellationToken);
        }

        public Task<List<ProcurementDalDto>> GetBySalePointIds(List<long> ids, CancellationToken cancellationToken)
        {

            return _context.Procurements
                    .Where(x => x.SalePoints
                                .Where(y=> ids.Contains(y.Id)) //если в списке SalePoints у конкретной закупки есть хотя бы 1 ТП, у которой Id подходит, выгружаем закупку
                                .FirstOrDefault() != null)?   
                    .ToListAsync(cancellationToken) ?? null;
        }



        public void EditRange(List<ProcurementDalDto> procurements)
        {
            foreach (var procurement in procurements) base.Update(procurement, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<List<ProcurementDalDto>> GetAll(CancellationToken cancellationToken)
        {
            return _context.Procurements.Include(x => x.Selections).ToListAsync(cancellationToken);
        }

        public async Task<List<ColorModelMetaDalDto>> GetProductStatisticQuery(long procurementsId, List<string> modelVendoreCodes, CancellationToken cancellationToken)
        {
            return await _context.ColorModelMetas.Include(q => q.Selection).Where(q => modelVendoreCodes.Contains(q.ModelVendorCodeSbs)   && q.Selection.ProcurementId == procurementsId).ToListAsync(cancellationToken);
        }

        public override async Task Add(ProcurementDalDto entity, CancellationToken cancellationToken)
        {
            await base.Add(entity, cancellationToken);
            entity.Selections.ForEach(x =>
            {
                x.EntityModificationInfo = entity.EntityModificationInfo;
                x.SelectionPurchaseSalePointKeys.ToList().ForEach(spKey => spKey.EntityModificationInfo = entity.EntityModificationInfo);
            });
        }

        public override async Task AddRange(IEnumerable<ProcurementDalDto> entities, CancellationToken cancellationToken)
        {
            await base.AddRange(entities, cancellationToken);
            foreach (var entity in entities)
            {
                entity.Selections.ForEach(x =>
                {
                    x.EntityModificationInfo = entity.EntityModificationInfo;
                    x.SelectionPurchaseSalePointKeys.ToList().ForEach(spKey => spKey.EntityModificationInfo = entity.EntityModificationInfo);
                });
            }
        }
    }
}