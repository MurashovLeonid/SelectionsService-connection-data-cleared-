using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Domain.Procurements;

namespace Superbrands.Selection.Infrastructure.DAL
{
    public class ProcurementKeySetDalDto : AuditableEntity
    {
        public long ProcurementId { get; set; }
        public int BuyerId { get; set; }
        public int PurchaseKeyId { get; set; }
        public int FinancialPlaningCenterId { get; set; }
        public virtual ProcurementDalDto Procurement { get; set; }
        public ProcurementKeySet ToDomain()
        {
            return new(ProcurementId, BuyerId, PurchaseKeyId, FinancialPlaningCenterId) { Id = Id };
        }
    }
}
