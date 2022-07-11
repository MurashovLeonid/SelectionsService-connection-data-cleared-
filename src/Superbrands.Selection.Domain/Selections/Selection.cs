using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Domain.Events;
using Superbrands.Selection.Domain.Logs;
using Superbrands.Selection.Domain.Logs.Messages;
using Superbrands.Selection.Domain.Procurements;
using Superbrands.Selection.Domain.SalePoints;

namespace Superbrands.Selection.Domain.Selections
{
    [DataContract]
    public class Selection : EntityBase, IEqualityComparer<Selection>
    {
        [DataMember] public long ProcurementId { get; internal set; }

        [DataMember] public SelectionStatus Status { get; internal set; } = SelectionStatus.InProgress;

        /// <summary>
        /// Закупщик
        /// </summary>
        [DataMember]
        public long BuyerId { get; internal set; }

        [DataMember]
        public IReadOnlyCollection<SalePointPriorityInfo> SalePointPriorityInfoCollection =>
            colorModelMetas.Where(x => x.ColorModelGroupKeys?.SalePointId != default)
                .GroupBy(x => x.ColorModelGroupKeys.SalePointId)
                .Select(x => new SalePointPriorityInfo(x.Key, x.ToList().Count > 1)).ToList();

        [DataMember]
        public ICollection<SelectionPurchaseSalePointKey> SelectionPurchaseSalePointKeys =>
            selectionPurchaseSalePointKeys?.AsReadOnly();
        internal List<SelectionPurchaseSalePointKey> selectionPurchaseSalePointKeys =
            new();
        
        [DataMember] public ICollection<ColorModelMeta> ColorModelMetas => colorModelMetas?.AsReadOnly();
        internal List<ColorModelMeta> colorModelMetas = new();

        [IgnoreDataMember] internal Procurement Procurement { get; private set; }

        internal List<LogEntry> logs = new();
        [DataMember] public IReadOnlyCollection<LogEntry> Logs => logs.AsReadOnly();

        public Selection()
        {
        }

        public Selection(long buyerId, long procurementId, SelectionStatus selectionStatus)
        {
            if (procurementId < 0)
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(procurementId));

            if (buyerId <= 0)
                throw new ArgumentOutOfRangeException(nameof(buyerId));

            ProcurementId = procurementId;
            Status = selectionStatus;
            BuyerId = buyerId;
        }

        public Selection(long buyerId, long procurementId, SelectionStatus status,
            List<SelectionPurchaseSalePointKey> selectionPurchaseSalePointKeys, List<ColorModelMeta> colorModelMetas) : this(
            buyerId, procurementId, status)
        {
            this.selectionPurchaseSalePointKeys = selectionPurchaseSalePointKeys;
            this.colorModelMetas = colorModelMetas;
        }

        public void AddProduct(ColorModelMeta product)
        {
            product.SelectionId = Id;
            colorModelMetas.Add(product);
            ReturnToWorkIfNeeded();
        }

        public void UpdateProduct(string sku, bool isCanceled)
        {
            var colorModelMetas = ColorModelMetas.Where(q => q.Sizes.Any(t => t.Sku == sku));

            foreach (var сolorModelMeta in colorModelMetas)
            {
                var size = сolorModelMeta.Sizes.First(q => q.Sku == sku);
                size.IsCanceled = isCanceled;
            }
        }

        
        private void ReturnToWorkIfNeeded()
        {
            if (Status == SelectionStatus.Agreed || Status == SelectionStatus.OnApproval || Status == SelectionStatus.SelectionIsEmpty) Status = SelectionStatus.InProgress;
        }

        public void AddPurchaseSalePointKeys(IEnumerable<SelectionPurchaseSalePointKey> keys)
        {
            selectionPurchaseSalePointKeys.AddRange(keys);
        }

        public void RemoveProduct(string modelVendorCodeSbs)
        {
            var productsToDelete = ColorModelMetas?
                .Where(x => x.ModelVendorCodeSbs == modelVendorCodeSbs).ToList();

            if (productsToDelete != null)
                foreach (var prod in productsToDelete)
                    colorModelMetas.Remove(prod);
        }

        public void RemoveColorModel(string colorModelVendorCodeSbs)
        {
            var productsToDelete = ColorModelMetas?.Where(x => x.ColorModelVendorCodeSbs == colorModelVendorCodeSbs).ToList();
            if (productsToDelete != null)
                foreach (var prod in productsToDelete)
                    colorModelMetas.Remove(prod);
        }

        public void AddColorModel(ColorModelMeta colorModelmeta)
        {
            var product = colorModelMetas.FirstOrDefault(x =>
                x.ModelVendorCodeSbs == colorModelmeta.ModelVendorCodeSbs &&
                x.ColorModelVendorCodeSbs == colorModelmeta.ColorModelVendorCodeSbs &&
                x.SelectionId == colorModelmeta.SelectionId);

            if (product == null)
                colorModelMetas.Add(colorModelmeta);
            ReturnToWorkIfNeeded();
        }

        public void SetStatus(SelectionStatus newStatus)
        {
            Status = newStatus;
        }

        public void ClearProducts()
        {
            foreach (var colorModelMeta in colorModelMetas)
            {
                colorModelMeta.Remove();
            }

            Status = SelectionStatus.InProgress;
        }

        /// <summary>
        /// Добавляет в отборку товары из списка, если их нет в этой отборке
        /// </summary>
        public void AddProducts(IEnumerable<ColorModelMeta> products)
        {
            var existingProductsIds = new HashSet<string>(ColorModelMetas.Select(p => p.ColorModelVendorCodeSbs));
            foreach (var sourceProduct in products)
                if (!existingProductsIds.Contains(sourceProduct.ColorModelVendorCodeSbs))
                    AddProduct(sourceProduct);

            if (products.Any())
            {
                Procurement.AddBrands(products.Select(p => p.ColorModelGroupKeys.BrandId).Distinct());
                ReturnToWorkIfNeeded();
            }
        }

        internal void SetProcurement(Procurement procurement)
        {
            if (procurement.Id == ProcurementId)
                Procurement = procurement;
            else
                throw new Exception($"Procurement id for current selection != {procurement.Id}");
        }

        public bool Equals(Selection x, Selection y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.colorModelMetas?.SequenceEqual(y.ColorModelMetas ?? Enumerable.Empty<ColorModelMeta>()) is null or true &&
                   x.selectionPurchaseSalePointKeys?.SequenceEqual(y.SelectionPurchaseSalePointKeys ?? Enumerable.Empty<SelectionPurchaseSalePointKey>()) is null or true
                   && x.Status == y.Status && x.BuyerId == y.BuyerId && x.ProcurementId == y.ProcurementId;
        }

        public int GetHashCode(Selection obj)
        {
            return HashCode.Combine(obj.ColorModelMetas, obj.selectionPurchaseSalePointKeys, obj.Status, obj.BuyerId,
                obj.ProcurementId);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(this, (Selection) obj);

        /// <inheritdoc />
        public override int GetHashCode() => GetHashCode(this);

        public void ChangeSizeCount(ColorModelMeta colorModelMeta)
        {
            ReturnToWorkIfNeeded();
            var meta = colorModelMetas.First(s => s.Id == colorModelMeta.Id);
            colorModelMetas.Remove(meta);
            colorModelMetas.Add(colorModelMeta);
        }
    }
}
