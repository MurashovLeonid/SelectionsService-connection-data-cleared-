using Mapster;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.Infrastructure.DAL
{
    public class SelectionPurchaseSalePointKeyDalDto : AuditableEntity
    {
        public long SelectionId { get; set; }
        public long SalePointId { get; set; }
        public long PurchaseKeyId { get; set; }
        public virtual SelectionDalDto Selection { get; set; }
        
        public SelectionPurchaseSalePointKey ToDomain()
        {
            return new(SelectionId, SalePointId, PurchaseKeyId) { Id = Id };
        }

        public static SelectionPurchaseSalePointKeyDalDto FromDomain(SelectionPurchaseSalePointKey key)
        {
            return key.Adapt<SelectionPurchaseSalePointKeyDalDto>();
        }
    }
}
