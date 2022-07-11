using MediatR;
using Superbrands.Selection.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Superbrands.Libs.RestClients.PartnersInfrastructure;
using Superbrands.Selection.Application.Requests;
using Superbrands.Selection.Infrastructure.DAL;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Libs.RestClients.Pim;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.SalePoints;
using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.Application.Procurement
{
    public class CreateProcurementForPartnerQueryHandler : IRequestHandler<CreateProcurementForPartnerQuery, IEnumerable<long>>
    {
        private readonly IProcurementRepository _procurementRepository;
        private readonly IProductMetaRepository _productMetaRepository;
        private readonly ISelectionRepository _selectionRepository;
        private readonly IPartnersInfrastructureSalepointpurchasekeysClient _salepointpurchasekeysClient;
        private readonly IMediator _mediator;
        private readonly IPartnersInfrastructureSalepointClient _salepointClient;
        private readonly IPimBbproductsClient _pimClient;

        public CreateProcurementForPartnerQueryHandler(IProcurementRepository repository, IPimBbproductsClient pimClient,
            IProductMetaRepository productMetaRepository, ISelectionRepository selectionRepository,
            IPartnersInfrastructureSalepointpurchasekeysClient salepointpurchasekeysClient, [NotNull] IMediator mediator,
            [NotNull] IPartnersInfrastructureSalepointClient salepointClient)
        {
            _pimClient = pimClient ?? throw new ArgumentNullException(nameof(pimClient));
            _productMetaRepository = productMetaRepository ?? throw new ArgumentNullException(nameof(productMetaRepository));
            _procurementRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _selectionRepository = selectionRepository ?? throw new ArgumentNullException(nameof(selectionRepository));
            _salepointpurchasekeysClient = salepointpurchasekeysClient ??
                                           throw new ArgumentNullException(nameof(salepointpurchasekeysClient));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _salepointClient = salepointClient ?? throw new ArgumentNullException(nameof(salepointClient));
        }

        public async Task<IEnumerable<long>> Handle(CreateProcurementForPartnerQuery request, CancellationToken cancellationToken)
        {
            var createdProcurementIds = new List<long>();
            foreach (var shortReq in request.CreateProcurementForPartnerRequests)
            {
                var sizesSkus = shortReq.SalePointRequestMetas.SelectMany(cm => cm.SizeRequestMetas).Select(sz => sz.SizeSku);
                var filterParams = new Dictionary<string, ICollection<object>> {{"ProductsSkus", sizesSkus.ToArray()}};
                var searchParams = new SearchProductsRequest() {Filters = filterParams};

                var pimProducts = await _pimClient.SearchAsync(searchParams, cancellationToken);
                var salePoints = shortReq.SalePointRequestMetas.Select(x => new Superbrands.Selection.Domain.SalePoints.SalePoint(x.SalePointId, true)).ToList();

                var salePointDtos = await _salepointClient.SalePoint_GetByIdsAsync(salePoints.Select(s => s.Id), cancellationToken);
                var counterpartyConditions =
                    await _mediator.Send(new CreateCounterpartyConditionQuery(shortReq.PartnerId, 
                            salePointDtos.Where(s => s.CounterpartyId.HasValue).Select(s => s.CounterpartyId.Value)),
                        cancellationToken);
                var procurement = new Domain.Procurements.Procurement(shortReq.ExternalKeyFrom1C, shortReq.SeasonCapsuleId,
                    shortReq.PartnerId, shortReq.ProcurementKind, shortReq.ProcurementStage, shortReq.ProcurementStatus,
                    salePoints, new Dictionary<long, decimal>(), counterpartyConditions);
                var procurementDal = ProcurementDalDto.FromDomain(procurement);
                await _procurementRepository.Add(procurementDal, cancellationToken);
                createdProcurementIds.Add(procurementDal.Id);
                await GenerateSelectionsBySalePointIds(shortReq.SalePointRequestMetas, pimProducts.Results, procurementDal.Id,
                    cancellationToken);
            }

            return createdProcurementIds;
        }

        private async Task GenerateSelectionsBySalePointIds(ICollection<SalePointRequestMeta> salePointRequestMetas,
            ICollection<ProductData> b2BPimProductDtos, long procurementId, CancellationToken cancellationToken)
        {
            var salePointPurchaseKeys =
                await _salepointpurchasekeysClient.SalePointPurchaseKeys_GetBySalePointsIdsAsync(
                    salePointRequestMetas.Select(x => x.SalePointId).ToList(), cancellationToken);
            var groupedResponses = salePointPurchaseKeys.GroupBy(b => b.BuyerId);

            foreach (var responseFromPartnersInfrastructure in groupedResponses)
            {
                var newSelectionEntity = new Domain.Selections.Selection(
                    responseFromPartnersInfrastructure.Key.GetValueOrDefault(), procurementId,
                    SelectionStatus.InProgress);

                var selectionDal = SelectionDalDto.FromDomain(newSelectionEntity);
                await _selectionRepository.Add(selectionDal, cancellationToken);
                var selection = selectionDal.ToDomain();

                var selectionPurchaseSalePointKeys = responseFromPartnersInfrastructure.Select(s
                    => new SelectionPurchaseSalePointKey(selection.Id, s.SalePointId, s.PurchaseKeyId)).ToList();
                selection.AddPurchaseSalePointKeys(selectionPurchaseSalePointKeys);
                await _selectionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

                var filteredSalePointsRequestMetas = salePointRequestMetas
                    .Where(s => selectionPurchaseSalePointKeys.Any(sp => sp.SalePointId == s.SalePointId)).ToList();

                var b2bProductFilteredBySalePoints = b2BPimProductDtos.Where(p => p.ColorLevel.Any(cl =>
                        cl.RangeSizeLevel.Any(sz =>
                            filteredSalePointsRequestMetas.Any(sss => sss.SizeRequestMetas.Any(szm => szm.SizeSku == sz.Sku)))))
                    .ToList();

                var filteredSizeRequestMetas = filteredSalePointsRequestMetas.SelectMany(x => x.SizeRequestMetas).ToList();
                await AddPimProductsToRepositoryAndToSelection(b2bProductFilteredBySalePoints, selection,
                    filteredSizeRequestMetas, cancellationToken);
            }
        }

        private async Task AddPimProductsToRepositoryAndToSelection(IEnumerable<ProductData> filteredByRangeSizes,
            Domain.Selections.Selection selection, List<SizeRequestMeta> sizeRequestMetas, CancellationToken cancellationToken)
        {
            foreach (var modelFromPim in filteredByRangeSizes)
            {
                foreach (var colorLevel in modelFromPim.ColorLevel)
                {
                    var sizes = colorLevel.RangeSizeLevel.Select(rsl => new Size(rsl.Sku,
                        sizeRequestMetas.FirstOrDefault(x => x.SizeSku == rsl.Sku).Count,
                        (int) rsl.Bwp.GetValueOrDefault(), 1)).ToList();

                    var colorModel = new ColorModelMeta(modelFromPim.ModelVendorCodeSbs, selection.Id,
                        colorLevel.ColorModelVendorCodeSbs, ColorModelStatus.None,
                        sizeRequestMetas.FirstOrDefault(x => x.SizeSku == sizes.FirstOrDefault()?.Sku)
                            .ColorModelPriority, sizes, modelFromPim.Currency);

                    await _productMetaRepository.Add(ColorModelMetaDalDto.FromDomain(colorModel), cancellationToken);
                    selection.AddColorModel(colorModel);
                }
            }

            await _productMetaRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}