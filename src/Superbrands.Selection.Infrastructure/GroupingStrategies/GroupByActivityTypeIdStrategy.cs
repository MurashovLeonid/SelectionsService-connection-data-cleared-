using Superbrands.Selection.Domain;
using System.Collections.Generic;
using System.Linq;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.Infrastructure.GroupingStrategies
{
    public class GroupByActivityTypeIdStrategy : IGroupingStrategy
    {
        public IEnumerable<SelectionGroup> GetGroupedSelections(IEnumerable<Domain.Selections.Selection> selections, 
        IEnumerable<long> memberIds, long partnerId, long seasonCapsuleId)
        {
            var result = selections.SelectMany(s => s.ColorModelMetas.GroupBy(cm => cm.ColorModelGroupKeys.ActivityTypeId).Select(sg => new SelectionGroup
            {
                KeyParameters = new KeyParameters
                {
                    MemberIds = memberIds,
                    PartnerId = partnerId,
                    SeasonCapsuleId = seasonCapsuleId,
                },
                SelectionId = s.Id,
                Meta = new ProductGroupMeta
                {
                    GroupKeyType = GroupKeyType.ActivityTypeId,
                    ProductsCount = sg.Select(a => a).Distinct().Count(),
                    AverageRrc = sg.Average(cm => cm.Sizes.Average(sz => sz.Rrc)),
                    AverageBwp = sg.Average(cm => cm.Sizes.Average(sz => sz.Bwp)),
                    ColorModelCount = sg.Count(),
                    SizesCount = sg.Sum(cm => cm.Sizes.Sum(sz => sz.Count)),
                }
            }));
            return result;
        }
    }
}
