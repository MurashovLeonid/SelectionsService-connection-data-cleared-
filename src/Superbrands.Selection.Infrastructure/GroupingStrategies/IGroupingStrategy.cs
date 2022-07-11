using System.Collections.Generic;
using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.Infrastructure.GroupingStrategies
{
    public interface IGroupingStrategy
    {
        IEnumerable<SelectionGroup> GetGroupedSelections(IEnumerable<Domain.Selections.Selection> selections, IEnumerable<long> 
        memberIds, long partnerId, long seasonCapsuleId);
    }
}
