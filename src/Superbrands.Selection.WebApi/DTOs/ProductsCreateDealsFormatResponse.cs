using System.Collections.Generic;
using System.Linq;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class ProductsCreateDealsFormatResponse
    {
        public IEnumerable<ProductsCreateDealsFormatDto> Products { get; set; }

        public ProductsCreateDealsFormatResponse(Domain.Selections.Selection selection)
        {
            Products = selection.ColorModelMetas.Select(x => new ProductsCreateDealsFormatDto()
            {
                ModelVendorCodeSbs = x.ModelVendorCodeSbs,
                ColorModelVendorCodeSbs = x.ColorModelVendorCodeSbs,
                SizesSkus = x.Sizes.Select(s=>s.Sku).ToList()
            });
        }
    }

    public class ProductsCreateDealsFormatDto
    {
        public string ModelVendorCodeSbs { get; set; }
        public string ColorModelVendorCodeSbs { get; set; }
        public List<string> SizesSkus{ get; set; }
    }
}