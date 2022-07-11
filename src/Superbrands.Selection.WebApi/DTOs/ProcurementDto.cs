using Superbrands.Selection.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Superbrands.Selection.Domain.Procurements;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class ProcurementDto
    {
        public long Id { get; set; }
        public long PartnerId { get; set; }
        public string ExternalKeyFrom1C { get; set; }
        public long SeasonCapsuleId { get; set; }
        public ProcurementKind Kind { get; set; }
        public ProcurementStatus Status { get; set; }
        public IEnumerable<SalePointDto> SalePoints { get; set; }
        public Dictionary<long, decimal> Brands { get; set; }
        
        public IEnumerable<ProcurementKeySetDto> ProcurementKeySets { get; set; }
        public bool? IsPreOrder { get; set; }
        public long CooperationId { get; internal set; }
        public IEnumerable<CounterpartyCondition> CounterpartyConditions { get; set; }
        public List<long> SbsTeamMembersIds { get; set; }
        public List<long> PartnerTeamMembersIds { get; set; }

        public ProcurementDto()
        {
        }

        public ProcurementDto([NotNull] ProcurementDalDto procurement)
        {
            if (procurement == null) throw new ArgumentNullException(nameof(procurement));
            Id = procurement.Id;
            
            SeasonCapsuleId = procurement.SeasonId;
            Kind = procurement.Kind;
            ExternalKeyFrom1C = procurement.ExternalKeyFrom1C;
            PartnerId = procurement.PartnerId;

            IsPreOrder = procurement.IsPreorder;
            Status = procurement.Status;
            SalePoints = procurement.SalePoints.Select(s => new SalePointDto(s));
            Brands = procurement.Brands;
            CounterpartyConditions = procurement.CounterpartyConditions;
            CooperationId = procurement.CooperationId;
            PartnerTeamMembersIds = procurement.PartnerTeamMembersIds;
            SbsTeamMembersIds = procurement.SbsTeamMembersIds;

        }
        public ProcurementDto([NotNull] Procurement procurement)
        {
            if (procurement == null) throw new ArgumentNullException(nameof(procurement));
            Id = procurement.Id;

            SeasonCapsuleId = procurement.SeasonId;
            Kind = procurement.Kind;
            ExternalKeyFrom1C = procurement.ExternalKeyFrom1C;
            PartnerId = procurement.PartnerId;
            IsPreOrder = procurement.IsPreorder;

            Status = procurement.Status;
            SalePoints = procurement.SalePoints.Select(s => new SalePointDto(s));
            Brands = procurement.Brands;
            CounterpartyConditions = procurement.CounterpartyConditions;
            CooperationId = procurement.CooperationId;
            SegmentId = procurement.SegmentId;
            PartnerTeamMembersIds = procurement.PartnerTeamMembersIds;
            SbsTeamMembersIds = procurement.SbsTeamMembersIds;
        }

        public long? SegmentId { get; set; }
    }
}