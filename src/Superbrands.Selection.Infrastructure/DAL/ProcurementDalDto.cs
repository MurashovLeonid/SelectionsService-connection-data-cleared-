using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Mapster;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Domain.Procurements;
using Superbrands.Selection.Domain.SalePoints;

namespace Superbrands.Selection.Infrastructure.DAL
{
    public class ProcurementDalDto : AggregateRootDALDTO<Procurement, ProcurementDalDto>
    {
        public long SeasonId { get; set; }
        public string ExternalKeyFrom1C { get; set; }
        public long PartnerId { get; set; }
        public ProcurementKind Kind { get; set; }
        public ProcurementStage Stage { get; set; }
        public ProcurementStatus Status { get; set; }
        public List<SalePoint> SalePoints { get; set; }
        public List<SelectionDalDto> Selections { get; set; }
        public List<ProcurementKeySetDalDto> ProcurementKeySets { get; set; }
        public List<CounterpartyCondition> CounterpartyConditions { get; set; }
        public Dictionary<long, decimal> Brands { get; set; }
        
        public bool? IsPreorder { get; set; }

        public bool IsApproved { get; set; }
        public long CooperationId { get; set; }
        public long SegmentId { get; set; }
        public List<long> SbsTeamMembersIds { get; set; }
        public List<long> PartnerTeamMembersIds { get; set; }
        
        
        public static ProcurementDalDto FromDomain(Domain.Selections.Selection selection) => ProcurementDalDto.FromDomain(selection.Procurement);
    }
}