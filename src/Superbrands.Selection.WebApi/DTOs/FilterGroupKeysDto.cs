using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class FilterGroupKeysDto
    {
        public int? DepartmentId { get; set; }
        public int? SalePointId { get; set; }
        public int? PurchaseKeyId { get; set; }
        public int? AssortmentGroupId { get; set; }
        public int? BrandId { get; set; }
        public int? ActivityId { get; set; }
        public int? ActivityTypeId { get; set; }

        public FilterGroupKeys ToDomain()
        {
            return new()
            {
                DepartmentId = DepartmentId,
                ActivityId = ActivityId,
                ActivityTypeId = ActivityTypeId,
                AssortmentGroupId = AssortmentGroupId,
                BrandId = BrandId,
                PurchaseKeyId = PurchaseKeyId,
                SalePointId = SalePointId
            };
        }
    }
}
