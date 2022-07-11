using System;
using System.Collections.Generic;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain.Abstractions;

namespace Superbrands.Selection.Domain
{
    public class Size : ValueObject, ICloneable, IEqualityComparer<Size>
    {
        public Size(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(sku));

            Sku = sku;
        }

        public Size(string sku, int count, double bwp, double rrc) : this (sku)
        {
            if (count < 0)
                throw new ArgumentException($"Value cannot be {count}", nameof(count));

            if (bwp < 0)
                throw new ArgumentException($"Value cannot be {bwp}", nameof(bwp));

            if (rrc < 0)
                throw new ArgumentException($"Value cannot be {rrc}", nameof(rrc));

            Bwp = bwp;
            Rrc = rrc;
            Count = count;
        }

        internal Size()
        {
        }

        public string Sku { get;  set; }
        public double Bwp { get;  set; }
        public double Rrc { get;  set; }
        public int Count { get;  set; }
        public bool IsCanceled { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Sku;
            yield return Bwp;
            yield return Rrc;
            yield return Count;
        }

        /// <inheritdoc />
        public object Clone()
        {
           return FastDeepCloner.DeepCloner.Clone(this);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(this, (Size) obj);

        /// <inheritdoc />
        public override int GetHashCode() => GetHashCode(this);

        public bool Equals(Size x, Size y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Sku == y.Sku && x.Bwp.Equals(y.Bwp) && x.Rrc.Equals(y.Rrc) && x.Count == y.Count && x.IsCanceled == y.IsCanceled;
        }

        public int GetHashCode(Size obj)
        {
            return HashCode.Combine(obj.Sku, obj.Bwp, obj.Rrc, obj.Count, obj.IsCanceled);
        }
    }
}