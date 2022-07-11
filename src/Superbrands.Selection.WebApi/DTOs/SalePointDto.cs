using Superbrands.Selection.Domain.SalePoints;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class SalePointDto
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public int SelectedProductsCount { get; set; }

        public SalePointDto()
        {
        }

        public SalePointDto(SalePoint salePoint)
        {
            Id = salePoint.Id;
            IsActive = salePoint.IsActive;
            SelectedProductsCount = 0;
        }

        public SalePoint ToDomain()
        {
            return new(Id, IsActive);
        }
    }
}