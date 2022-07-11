using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Force.DeepCloner;
using JetBrains.Annotations;
using MediatR;
using Superbrands.Bus.Contracts.CSharp.MsSelections.Selections;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Libs.RestClients.Pim;
using Superbrands.Selection.Domain.Events;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Application.Products
{
    public class ChangeProductQuantityQueryHandler : IRequestHandler<ChangeProductQuantityQuery>
    {
        private readonly IProductMetaRepository _productRepository;
        private readonly ISelectionRepository _selectionRepository;
        private readonly IProcurementRepository _procurementRepository;
        private readonly IPimBbproductsClient _pimClient;

        public ChangeProductQuantityQueryHandler(IProductMetaRepository productRepository,
            [NotNull] IProcurementRepository procurementRepository,
            [NotNull] ISelectionRepository selectionRepository, [NotNull] IPimBbproductsClient pimClient)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _selectionRepository = selectionRepository ?? throw new ArgumentNullException(nameof(selectionRepository));
            _pimClient = pimClient ?? throw new ArgumentNullException(nameof(pimClient));
            _procurementRepository = procurementRepository ?? throw new ArgumentNullException(nameof(procurementRepository));
        }
        
        private async Task<IEnumerable<RangeSizeLevel>> GetSizesFromPim(IEnumerable<string> skus, CancellationToken cancellationToken)
        {
            if (!skus.Any())
                return new List<RangeSizeLevel>();

            var filterParams = new Dictionary<string, ICollection<object>> {{"ProductsSkus", skus.ToArray()}};
            var searchParams = new SearchProductsRequest() { Filters = filterParams };

            var pimProducts = await _pimClient.SearchAsync(searchParams, cancellationToken);
            var sizesFromPim = pimProducts.Results.SelectMany(p => p.ColorLevel).SelectMany(cl => cl.RangeSizeLevel).Where(rs => skus.Contains(rs.Sku));

            if (!sizesFromPim.Any() || sizesFromPim.Count() != skus.Count())
            {
                var notFoundSkus = skus.Except(sizesFromPim.Select(x => x.Sku));
                throw new Exception($"Пришедших в запросе Sku размеров нет в пим. Не найдены: {string.Join(",", notFoundSkus)}");
            }

            return sizesFromPim.ToList();
        }

        public async Task<Unit> Handle(ChangeProductQuantityQuery request, CancellationToken cancellationToken)
        {
            var sizesSkus = request.SizeInfos.Select(s => s.Sku).Distinct().ToList();
            var sizesFromPim = await GetSizesFromPim(sizesSkus, cancellationToken);
            var sizeInfosBySku = request.SizeInfos.ToDictionary(s => s.Sku);
            foreach (var rangeSizeLevel in sizesFromPim)
            {
                sizeInfosBySku[rangeSizeLevel.Sku].SetBwp(rangeSizeLevel.Bwp);
            }
            var colorModelMeta = await _productRepository.ChangeSizeChartAndSizesCount(request.ColorModelMetaId,
                request.SizeInfos, request.SizeChartCount, cancellationToken, request.SizeChartId);
            
            var selectionDal = await _selectionRepository.GetById(colorModelMeta.SelectionId, cancellationToken);
            var domainProcurement = selectionDal.Procurement.ToDomain<ProcurementUpdatedEvent, Domain.Procurements.Procurement>();
            var cloneProcurement = domainProcurement.DeepClone();
            var selection = selectionDal.ToDomain(domainProcurement);
            selection.ChangeSizeCount(colorModelMeta.ToDomain());
            var procurementDal = ProcurementDalDto.FromDomain(selection);
            var currentProcurement = procurementDal.ToDomain<ProcurementUpdatedEvent, Domain.Procurements.Procurement>();

            procurementDal.ClearDomainEvents();
            procurementDal.AddEvent(new ProcurementUpdatedEvent(currentProcurement, cloneProcurement));
            
            
            foreach (var selectionMapped in procurementDal.Selections)
            {
                if (selectionMapped.EntityModificationInfo == null)
                {
                    selectionMapped.EntityModificationInfo = selectionDal.EntityModificationInfo;
                }
                foreach (var modelMetaDalDto in selectionMapped.ColorModelMetas)
                {
                    if (modelMetaDalDto.EntityModificationInfo == null)
                    {
                        modelMetaDalDto.EntityModificationInfo = selectionDal.ColorModelMetas
                            .FirstOrDefault(c => c.Id == modelMetaDalDto.Id)?.EntityModificationInfo;
                    }
                }
            }
            if (procurementDal.EntityModificationInfo == null)
                procurementDal.EntityModificationInfo = selectionDal.EntityModificationInfo;
            
            await _procurementRepository.Update(procurementDal, cancellationToken);
            await _selectionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}