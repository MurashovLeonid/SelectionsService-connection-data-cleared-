using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Domain.Events;
using Superbrands.Selection.Domain.SalePoints;

[assembly: InternalsVisibleTo("Superbrands.Selection.Infrastructure")]

namespace Superbrands.Selection.Domain.Procurements
{
    public class Procurement : AggregateRootBase, ICloneable, IEqualityComparer<Procurement>
    {
        public string ExternalKeyFrom1C { get; internal set; }
        public long PartnerId { get; internal set; }
        public long SeasonId { get; internal set; }
        
        public ProcurementKind Kind { get; internal set; }
        public ProcurementStatus Status { get; internal set; }
        public long CooperationId { get; internal set; }
        public long SegmentId { get; internal set; }
        
        /// <summary>
        /// Какие могут участвовать
        /// </summary>
        public List<SalePoint> SalePoints { get; internal set; }

        public IReadOnlyCollection<Selections.Selection> Selections => _selections.AsReadOnly();
        internal List<Selections.Selection> _selections { get; set; } = new();

        public IEnumerable<ProcurementKeySet> ProcurementKeySets { get; set; } = new List<ProcurementKeySet>();

        public IEnumerable<CounterpartyCondition> CounterpartyConditions { get; set; } =
            new List<CounterpartyCondition>();
        
        public bool? IsPreorder { get; internal set; }
        public Dictionary<long, decimal> Brands { get; internal set; }
        public List<long> SbsTeamMembersIds { get; internal set; }
        public List<long> PartnerTeamMembersIds { get; internal set; }
        public bool IsApproved() => Selections?.All(x => x?.Status == SelectionStatus.Agreed) ?? false;

        internal Procurement(bool isReadOnly = false)
        {
        }
        
        public Procurement(string externalKeyFrom1C, long seasonId, long partnerId, ProcurementKind kind,
            ProcurementStage stage, ProcurementStatus status, List<SalePoint> salePoints, Dictionary<long, decimal> brands,
            IEnumerable<CounterpartyCondition> counterpartyConditions,
            long segmentId = 0, bool? isPreorder = null)
        {
            if (!salePoints?.Any() ?? true) throw new ArgumentNullException(nameof(salePoints));

            if (seasonId == 0) throw new ArgumentOutOfRangeException(nameof(seasonId));
            if (string.IsNullOrWhiteSpace(externalKeyFrom1C)) throw new ArgumentOutOfRangeException(nameof(externalKeyFrom1C));

            if (partnerId <= 0) throw new ArgumentOutOfRangeException(nameof(partnerId));

            if (!Enum.IsDefined(typeof(ProcurementKind), kind))
                throw new InvalidEnumArgumentException(nameof(kind), (int) kind, typeof(ProcurementKind));

            if (!Enum.IsDefined(typeof(ProcurementStage), stage))
                throw new InvalidEnumArgumentException(nameof(stage), (int) stage, typeof(ProcurementStage));

            if (!Enum.IsDefined(typeof(ProcurementStatus), status))
                throw new InvalidEnumArgumentException(nameof(status), (int) status, typeof(ProcurementStatus));

            SeasonId = seasonId;
            PartnerId = partnerId;
            Kind = kind;
            Status = status;
            ExternalKeyFrom1C = externalKeyFrom1C;
            SalePoints = salePoints;
            Brands = brands ?? new Dictionary<long, decimal>();
            CounterpartyConditions = counterpartyConditions;
            IsPreorder = isPreorder;

            //CooperationId = cooperationId;
            SegmentId = segmentId;
            AddEvent(new ProcurementCreatedEvent(this));
        }

        public void EditSeasonCapsuleStatus(bool? isPreorder)
        {
            if (isPreorder == null) throw new ArgumentNullException(nameof(isPreorder));
            IsPreorder = isPreorder;
        }

        public void Delete()
        {
            if (Id != default)
                AddEvent(new ProcurementDeletedEvent(this));
        }

        public void AddSalePoints(List<SalePoint> salePoints)
        {
            SalePoints.AddRange(salePoints);
        }
        public void AddSelection(Selections.Selection selection)
        {
            if (selection == null)
                throw new ArgumentNullException(nameof(selection));

            selection.SetProcurement(this);
            _selections.Add(selection);
        }

        public void AddBrands(IEnumerable<long> brandIds)
        {
            foreach (var brandId in brandIds)
                if (!Brands.ContainsKey(brandId))
                    Brands.Add(brandId, 0);
        }

        public void SetBrandDiscount(in int brandId, in decimal discount)
        {
            if (Brands.ContainsKey(brandId)) Brands[brandId] = discount;
        }

        /// <inheritdoc />
        public object Clone()
        {
            return FastDeepCloner.DeepCloner.Clone(this);
        }

        public bool Equals(Procurement x, Procurement y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.SeasonId == y.SeasonId && x.PartnerId == y.PartnerId && x.Kind == y.Kind &&
                   x.Status == y.Status && x.IsPreorder == y.IsPreorder && Equals(x.Brands, y.Brands);
        }

        public int GetHashCode(Procurement obj)
        {
            return HashCode.Combine(obj.SeasonId, obj.PartnerId, (int) obj.Kind, (int) obj.Status,
                obj.IsPreorder, obj.Brands);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(this, (Procurement) obj);

        /// <inheritdoc />
        public override int GetHashCode() => GetHashCode(this);

        public void Edit(Procurement procurement)
        {
            if (procurement == null) throw new ArgumentNullException(nameof(procurement));
            var original = Clone();
            SeasonId = procurement.SeasonId;
            PartnerId = procurement.PartnerId;
            Kind = procurement.Kind;
            Status = procurement.Status;
            SalePoints = procurement.SalePoints;
            Brands = procurement.Brands;
            IsPreorder = procurement.IsPreorder;
            AddEvent(new ProcurementUpdatedEvent(this, (Procurement) original));
        }

        public void DeleteSelection(long selectionId)
        {
            var selection = Selections.First(s => s.Id == selectionId);
            _selections.Remove(selection);
        }

        public void SetTeams(List<long> partnerTeam, List<long> sbsTeam)
        {
            PartnerTeamMembersIds = partnerTeam;
            SbsTeamMembersIds = sbsTeam;
        }
        
    }
}