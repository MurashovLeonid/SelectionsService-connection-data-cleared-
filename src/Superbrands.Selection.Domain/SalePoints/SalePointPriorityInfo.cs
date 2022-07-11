using System;

namespace Superbrands.Selection.Domain.SalePoints
{
    public class SalePointPriorityInfo
    {
        public bool IsAllowToDeletePriorityB { get; internal set; }
        public long SalePointId { get; internal set; }

        public SalePointPriorityInfo(long salePointId, bool isAllowToDeletePriorityB)
        {
            if (salePointId <= 0)
                throw new ArgumentException(nameof(salePointId));

            SalePointId = salePointId;
            IsAllowToDeletePriorityB = isAllowToDeletePriorityB;
        }
    }
}
