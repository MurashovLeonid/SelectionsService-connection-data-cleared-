using System;
using System.Collections.Generic;
using Superbrands.Libs.DDD.Abstractions;

namespace Superbrands.Selection.Domain.Selections
{
    public class SelectionPurchaseSalePointKey : EntityBase, IEqualityComparer<SelectionPurchaseSalePointKey>
    {
        public long SelectionId { get; internal set; }
        public long SalePointId { get; internal set; }
        public long PurchaseKeyId { get; internal set; }

        internal SelectionPurchaseSalePointKey() { }

        public SelectionPurchaseSalePointKey(long selectionId, long salePointId, long purchaseKeyId)
        {
            if (selectionId < 0)
                throw new ArgumentOutOfRangeException(nameof(selectionId));

            if (salePointId <= 0)
                throw new ArgumentOutOfRangeException(nameof(salePointId));

            if (purchaseKeyId <= 0)
                throw new ArgumentOutOfRangeException(nameof(purchaseKeyId));

            SelectionId = selectionId;
            SalePointId = salePointId;
            PurchaseKeyId = purchaseKeyId;
        }

        public bool Equals(SelectionPurchaseSalePointKey x, SelectionPurchaseSalePointKey y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.SelectionId == y.SelectionId && x.SalePointId == y.SalePointId && x.PurchaseKeyId == y.PurchaseKeyId;
        }

        public int GetHashCode(SelectionPurchaseSalePointKey obj)
        {
            return HashCode.Combine(obj.SelectionId, obj.SalePointId, obj.PurchaseKeyId);
        }

        /// <inheritdoc />
        public override int GetHashCode() => GetHashCode(this);

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(this, (SelectionPurchaseSalePointKey) obj);
    }
}
