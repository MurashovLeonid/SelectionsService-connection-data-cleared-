using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Force.DeepCloner;
using JetBrains.Annotations;
using MediatR;
using Superbrands.Libs.RestClients.Pim;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.Events;
using Superbrands.Selection.Domain.Selections;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Application.Selection
{
    //ToDo: нужно переписывать
    internal class AddProductsToSelectionQueryHandler : IRequestHandler<AddProductsToSelectionQuery, Unit>
    {
        private const int ActivityTypeId = 1;
        private const int ActivityId = 1;

        private readonly ISelectionRepository _selectionsRepository;
        private readonly IPimBbproductsClient _pimClient;
        private readonly IProcurementRepository _procurementRepository;
        private readonly IColorModelMetaRepository _colorModelMetaRepository;

        public AddProductsToSelectionQueryHandler(ISelectionRepository repository, IPimBbproductsClient pimClient,
            [NotNull] IProcurementRepository procurementRepository, IColorModelMetaRepository colorModelMetaRepository)
        {
            _selectionsRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _pimClient = pimClient ?? throw new ArgumentNullException(nameof(pimClient));
            _procurementRepository = procurementRepository ?? throw new ArgumentNullException(nameof(procurementRepository));
            _colorModelMetaRepository = colorModelMetaRepository ?? throw new ArgumentNullException(nameof(colorModelMetaRepository));
        }

        public async Task<Unit> Handle(AddProductsToSelectionQuery request, CancellationToken cancellationToken)
        {
            var filters = new Dictionary<string, ICollection<object>>
            {
                {"ModelVendorCodeSbs", request.Products.Select(p => p.ModelVendorCodeSbs).Cast<object>().ToArray()}
            };

            var elasticSearchRequest = new SearchProductsRequest {Filters = filters};
            var result = await _pimClient.SearchAsync(elasticSearchRequest, cancellationToken);
            var products = result.Results;

            var seasonIds = products.SelectMany(s => s.Seasons.Select(r => r.Id)).ToList();         
            var purchaseKeyIds = products.Select(s => (long) s.PurchaseKeyId).ToList();

            var selections = await _selectionsRepository
                .GetSelectionsBySalePointIdAndPurchaseKeyIds(request.SalePointIds, purchaseKeyIds, seasonIds, request.ProcurementId, cancellationToken);

            var colorSBSByProductSBS = request.Products.GroupBy(p => p.ModelVendorCodeSbs)
                .ToDictionary(c => c.Key, c => new HashSet<string>(c.First().ColorModelVendorCodes));

            ProcurementDalDto procurementDal = null;
            Domain.Procurements.Procurement cloneProcurement = null;
            foreach (var selection in selections)
            {
                var colorModels = products
                     .Where(p => selection.SelectionPurchaseSalePointKeys.Any(s => s.PurchaseKeyId == p.PurchaseKeyId) &&
                                p.Seasons.Any(s => s.Id == selection.Procurement.SeasonId))
                    .SelectMany(pm => pm.ColorLevel
                        .Where(cl => colorSBSByProductSBS.ContainsKey(pm.ModelVendorCodeSbs) &&
                                     colorSBSByProductSBS[pm.ModelVendorCodeSbs].Contains(cl.ColorModelVendorCodeSbs))
                        .Select(cl => new {Product = pm, ColorLevel = cl})
                        .Select(cl => new ColorModelMeta(pm.ModelVendorCodeSbs, selection.Id,
                            cl.ColorLevel.ColorModelVendorCodeSbs, Domain.Enums.ColorModelStatus.None,
                            Domain.Enums.ColorModelPriority.None, new List<Size>(), cl.Product.BwpCurrency)
                        {
                            ColorModelGroupKeys = new ColorModelGroupKeys(pm.PurchaseKeyId, cl.ColorLevel.AssortmentGroupId,
                                pm.BrandId.Value, ActivityTypeId, ActivityId,
                                selection.SelectionPurchaseSalePointKeys
                                    .FirstOrDefault(spd => (int) spd.PurchaseKeyId == pm.PurchaseKeyId).SalePointId)
                        }));

                foreach (var cm in colorModels)
                {
                    var colorModelMeta = selection.ColorModelMetas.FirstOrDefault(x =>
                        x.ColorModelVendorCodeSbs == cm.ColorModelVendorCodeSbs && x.SelectionId == cm.SelectionId &&
                        x.ModelVendorCodeSbs == cm.ModelVendorCodeSbs);
                    // if (colorModelMeta != null)
                    //     await UpdateColorModelMeta(colorModelMeta.ToDomain(), cm, cancellationToken);
                }

                var domainProcurement = selection.Procurement.ToDomain<ProcurementUpdatedEvent, Domain.Procurements.Procurement>();
                cloneProcurement = domainProcurement.DeepClone();
                var domainSelection = selection.ToDomain(domainProcurement);
                
                domainSelection.AddProducts(colorModels);
                procurementDal = ProcurementDalDto.FromDomain(domainSelection);

                
                // foreach (var procurementSelection in currentProcurement.Selections)
                // {
                //     foreach (var colorModelMeta in procurementSelection.ColorModelMetas)
                //     {
                //         colorModelMeta.
                //     }
                // }
                foreach (var selectionMapped in procurementDal.Selections)
                {
                    var originalSelection = selections.FirstOrDefault(s => s.Id == selectionMapped.Id);
                    foreach (var salePointKey in selectionMapped.SelectionPurchaseSalePointKeys)
                    {
                        if (salePointKey.EntityModificationInfo == null)
                            salePointKey.EntityModificationInfo = originalSelection?.SelectionPurchaseSalePointKeys
                                ?.FirstOrDefault(x => x.Id == salePointKey.Id)?.EntityModificationInfo;
                    }
                    if (selectionMapped.EntityModificationInfo == null)
                    {
                        selectionMapped.EntityModificationInfo = originalSelection?.EntityModificationInfo;
                    }
                    foreach (var modelMetaDalDto in selectionMapped.ColorModelMetas)
                    {
                        if (modelMetaDalDto.EntityModificationInfo == null)
                        {
                            
                            modelMetaDalDto.EntityModificationInfo = originalSelection?.ColorModelMetas?
                                .FirstOrDefault(c => c.Id == modelMetaDalDto.Id)?.EntityModificationInfo;
                        }
                    }
                }


            }

            if (procurementDal != null)
            {
                var currentProcurement =
                    procurementDal.ToDomain<ProcurementUpdatedEvent, Domain.Procurements.Procurement>();
                _procurementRepository.Update(procurementDal);
                // await _procurementRepository.Update(procurementDal, cancellationToken); todo починить этот метод!! при его использовании не сохраняются свойства в ColorModelGroupKeys
                procurementDal.ClearDomainEvents();
                procurementDal.AddEvent(new ProcurementUpdatedEvent(currentProcurement, cloneProcurement));
            }

            await _selectionsRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        private async Task UpdateColorModelMeta(ColorModelMeta colorModelMetaEntity, ColorModelMeta colorModelMetaForEdit,
            CancellationToken cancellationToken)
        {
            colorModelMetaEntity.ColorModelGroupKeys = colorModelMetaForEdit.ColorModelGroupKeys;
            await _colorModelMetaRepository.Update(ColorModelMetaDalDto.FromDomain(colorModelMetaEntity), cancellationToken);
        }
    }
}