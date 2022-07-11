using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain.Enums;

namespace Superbrands.Selection.Domain.Selections
{
    [DataContract]
    public class ColorModelMeta : EntityBase, IEqualityComparer<ColorModelMeta>
    {
        [DataMember] public string ModelVendorCodeSbs { get; protected set; }

        [DataMember] public long SelectionId { get; internal set; }

        [DataMember] public string ColorModelVendorCodeSbs { get; protected set; }

        [DataMember] public List<Size> Sizes { get; set; }

        [DataMember] public int SizeChartId { get; set; }

        [DataMember] public ColorModelStatus ColorModelStatus { get; protected set; }

        [DataMember] public ColorModelPriority ColorModelPriority { get; protected set; }

        [DataMember] public int SizeChartCount { get; set; }
        [DataMember] public string Currency { get; protected set; }

        [DataMember] public ColorModelGroupKeys ColorModelGroupKeys { get; set; }

        protected ColorModelMeta()
        {
            ColorModelStatus = ColorModelStatus.New;
        }

        public ColorModelMeta(string modelVendorCodeSbs, long selectionId, string colorModelVendorCodeSbs,
            ColorModelStatus colorModelStatus,
            ColorModelPriority colorModelPriority, string currency) : this()
        {
            if (selectionId <= 0) throw new ArgumentException("B2bProductId has wrong value", nameof(selectionId));
            if (string.IsNullOrEmpty(colorModelVendorCodeSbs))
                throw new ArgumentException("Value cannot be null or empty.", nameof(colorModelVendorCodeSbs));
            if (string.IsNullOrEmpty(modelVendorCodeSbs))
                throw new ArgumentException("Value cannot be null or empty.", nameof(modelVendorCodeSbs));


            ModelVendorCodeSbs = modelVendorCodeSbs;
            SelectionId = selectionId;
            ColorModelVendorCodeSbs = colorModelVendorCodeSbs;
            ColorModelStatus = colorModelStatus;
            ColorModelPriority = colorModelPriority;
            Currency = currency;
        }

        public ColorModelMeta(string modelVendorCodeSbs, long selectionId, string colorModelVendorCodeSbs,
            ColorModelStatus colorModelStatus,
            ColorModelPriority colorModelPriority, List<Size> sizes, string currency) : this(modelVendorCodeSbs, selectionId,
            colorModelVendorCodeSbs,
            colorModelStatus, colorModelPriority, currency)
        {
            Sizes = sizes ?? throw new ArgumentNullException(nameof(sizes));
        }

        public void ChangeColorModelStatus(ColorModelStatus colorModelStatus)
        {
            ColorModelStatus = colorModelStatus;
        }

        public override bool Equals(object obj) => Equals(this, obj);

        public override int GetHashCode() => GetHashCode(this);

        private bool Equals(ColorModelMeta x, object y)
        {
            if (y is ColorModelMeta meta)
                return Equals(x, meta);
            return false;
        }

        public bool Equals(ColorModelMeta x, ColorModelMeta y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            var colorModelGroupKeysEquality = x.ColorModelGroupKeys?.Equals(y.ColorModelGroupKeys);

            return x.ModelVendorCodeSbs == y.ModelVendorCodeSbs && x.SelectionId == y.SelectionId &&
                   x.ColorModelVendorCodeSbs == y.ColorModelVendorCodeSbs &&
                   x.Sizes?.SequenceEqual(y.Sizes, x.Sizes.FirstOrDefault()) is true or null &&
                   x.SizeChartId == y.SizeChartId && x.ColorModelStatus == y.ColorModelStatus &&
                   x.ColorModelPriority == y.ColorModelPriority && x.SizeChartCount == y.SizeChartCount &&
                   x.Currency == y.Currency && (colorModelGroupKeysEquality is true or null);
        }

        public int GetHashCode(ColorModelMeta obj)
        {
            var hashCode = new HashCode();
            hashCode.Add(obj.ModelVendorCodeSbs);
            hashCode.Add(obj.SelectionId);
            hashCode.Add(obj.ColorModelVendorCodeSbs);
            hashCode.Add(obj.Sizes);
            hashCode.Add(obj.SizeChartId);
            hashCode.Add((int) obj.ColorModelStatus);
            hashCode.Add((int) obj.ColorModelPriority);
            hashCode.Add(obj.SizeChartCount);
            hashCode.Add(obj.Currency);
            hashCode.Add(obj.ColorModelGroupKeys);
            return hashCode.ToHashCode();
        }

        public void Remove()
        {
            Removed = true;
        }

        public bool Removed { get; private set; }
    }
}