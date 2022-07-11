using Superbrands.Selection.Domain.Enums;
using System.Collections.Generic;

namespace Superbrands.Selection.Application.Requests
{
    public class CreateProcurementForPartnerRequest
    {
        public string ExternalKeyFrom1C { get; set; }
        public ProcurementKind ProcurementKind { get; set; }
        public ProcurementStage ProcurementStage { get; set; }
        public ProcurementStatus ProcurementStatus { get; set; }
        public int PartnerId { get; set; }
        public long SeasonCapsuleId { get; set; }
        public ICollection<long> BrandIds { get; set; }
       public ICollection<SalePointRequestMeta> SalePointRequestMetas { get; set; }
    }

    public class SalePointRequestMeta
    {
        public long SalePointId { get; set; }
        public ICollection<SizeRequestMeta> SizeRequestMetas { get; set; }
    }

    public class SizeRequestMeta
    {
        public ColorModelPriority ColorModelPriority { get; set; }
        public string SizeSku { get; set; }
        public int Count { get; set; }
    }
}
