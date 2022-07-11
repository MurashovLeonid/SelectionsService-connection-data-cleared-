using MediatR;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Infrastructure;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.GroupingStrategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.Application.Procurement
{
    internal class GroupSelectionsQueryHandler : IRequestHandler<GroupSelectionsQuery, IEnumerable<SelectionGroup>>
    {
        private readonly IProcurementRepository _repository;

        public GroupSelectionsQueryHandler(IProcurementRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SelectionGroup>> Handle(GroupSelectionsQuery request, CancellationToken cancellationToken)
        {
            var filteredSelections =
                _repository.GetFilteredSelections(request.ProcurementId, request.ResponseFromBt, cancellationToken);
            var procurementData = filteredSelections.FirstOrDefault()?.Procurement;

            if (procurementData == default)
                return Enumerable.Empty<SelectionGroup>();

                var groupingStrategy = GetGroupingStrategy(request.GroupKeyType);
            var selectionGroups = groupingStrategy.GetGroupedSelections(filteredSelections.Select(x => x.ToDomain()).ToList(),
                request.ResponseFromBt.MemberIds, procurementData.PartnerId, procurementData.SeasonId);

            return selectionGroups;
        }

        private static IGroupingStrategy GetGroupingStrategy(GroupKeyType groupKeyType)
        {
            switch (groupKeyType)
            {
                case GroupKeyType.PurchaseKeyId:
                    return new GroupByPurchaseKeyIdStrategy();

                case GroupKeyType.SalePointId:
                    return new GroupBySalePointIdStrategy();

                case GroupKeyType.AssortmentGroupId:
                    return new GroupByAssortmentGroupIdStrategy();

                case GroupKeyType.BrandId:
                    return new GroupByBrandIdStrategy();

                case GroupKeyType.ActivityId:
                    return new GroupByActivityIdStrategy();

                case GroupKeyType.ActivityTypeId:
                    return new GroupByActivityTypeIdStrategy();

                default: throw new ArgumentException("Group key type is not found", nameof(groupKeyType));
            }
        }
    }
}