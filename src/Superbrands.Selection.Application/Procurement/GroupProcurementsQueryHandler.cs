using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.Procurements;
using Superbrands.Selection.Domain.Selections;
using Superbrands.Selection.Infrastructure;
using Superbrands.Selection.Infrastructure.Abstractions;

namespace Superbrands.Selection.Application.Procurement
{
    internal class GroupProcurementsQueryHandler : IRequestHandler<GroupProcurementsQuery, IEnumerable<ProcurementGroup>>
    {
        private readonly IProcurementRepository _repository;

        public GroupProcurementsQueryHandler(IProcurementRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<IEnumerable<ProcurementGroup>> Handle(GroupProcurementsQuery request, CancellationToken cancellationToken)
        {
            var groupedProcurements =  _repository.GetFilteredProcurements(request.ResponseFromBt, cancellationToken);

            var productsGroupedByProcurementId = groupedProcurements.SelectMany(pr => pr.Selections.SelectMany(sel => sel.ColorModelMetas.GroupBy(fp => pr)));

            var listOfProcurementGroupMetas = productsGroupedByProcurementId
                .Select(pg => new ProcurementGroup()
                {
                    ProcurementId = (int)pg.Key.Id,
                    KeyParameters = new KeyParameters
                    {
                        MemberIds = request.ResponseFromBt.MemberIds,
                        PartnerId = pg.Key.PartnerId,
                        SeasonCapsuleId = pg.Key.SeasonId,
                    },
                    Meta = new ProductGroupMeta()
                    {
                        AverageBwp = pg.Average(cm => cm.Sizes.Average(sz => sz.Bwp)),
                        AverageRrc = pg.Average(cm => cm.Sizes.Average(sz => sz.Rrc)),
                        ProductsCount = pg.Count(),
                        ColorModelCount = pg.Count(),
                        SizesCount = pg.Sum(cm => cm.Sizes.Count)
                    }
                }).ToList();

            return listOfProcurementGroupMetas;
        }
    }
}
