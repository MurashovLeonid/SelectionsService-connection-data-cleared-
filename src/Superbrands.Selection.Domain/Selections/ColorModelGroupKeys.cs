using System;
using System.Collections.Generic;
using Superbrands.Libs.DDD.Abstractions;

namespace Superbrands.Selection.Domain.Selections
{
    public class ColorModelGroupKeys : ValueObject, IEqualityComparer<ColorModelGroupKeys>
    {
        public long SalePointId { get; internal set; }
        public long PurchaseKeyId { get; internal set; }
        public long AssortmentGroupId { get; internal set; }
        public long BrandId { get; internal set; }
        public int ActivityId { get; internal set; }
        public int ActivityTypeId { get; internal set; }

        public ColorModelGroupKeys(long purchaseKeyId, long assortmentGroupId, long brandId, int activityId, int activityTypeId, long salePointId)
        {
            if (salePointId < 0)
                throw new ArgumentException("Value cannot be 0.", nameof(salePointId));
            if (purchaseKeyId <= 0)
                throw new ArgumentException("Value cannot be 0.", nameof(purchaseKeyId));
            // if (assortmentGroupId <= 0)
            //     throw new ArgumentException("Value cannot be 0.", nameof(assortmentGroupId)); todo раскоментить когда пим начнет их возвращать
            if (brandId < 0)
                throw new ArgumentException("Value cannot be 0.", nameof(brandId));
            if (activityId < 0)
                throw new ArgumentException("Value cannot be 0.", nameof(activityId));
            if (activityTypeId < 0)
                throw new ArgumentException("Value cannot be 0.", nameof(activityTypeId));

            SalePointId = salePointId;
            ActivityId = activityId;
            ActivityTypeId = activityTypeId;
            BrandId = brandId;
            AssortmentGroupId = assortmentGroupId;
            PurchaseKeyId = purchaseKeyId;
        }

        protected ColorModelGroupKeys()
        {
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return SalePointId;
            yield return ActivityId;
            yield return ActivityTypeId;
            yield return BrandId;
            yield return AssortmentGroupId;
            yield return PurchaseKeyId;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(this, (ColorModelGroupKeys) obj);

        /// <inheritdoc />
        public override int GetHashCode() => GetHashCode(this);

        public bool Equals(ColorModelGroupKeys x, ColorModelGroupKeys y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.SalePointId == y.SalePointId && x.PurchaseKeyId == y.PurchaseKeyId && x.AssortmentGroupId == y.AssortmentGroupId && x.BrandId == y.BrandId && x.ActivityId == y.ActivityId && x.ActivityTypeId == y.ActivityTypeId;
        }

        public int GetHashCode(ColorModelGroupKeys obj)
        {
            return HashCode.Combine(obj.SalePointId, obj.PurchaseKeyId, obj.AssortmentGroupId, obj.BrandId, obj.ActivityId, obj.ActivityTypeId);
        }
    }
}
