using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Superbrands.Libs.RestClients.Pim;

namespace Superbrands.Selection.WebApi.DTOs
{
    [DataContract]
    public class ProductsResponse
    {
        public ProductsResponse(string selectionName, IEnumerable<Product> products, HashSet<string> sizeSkusInSelection)
        {
            SelectionName = selectionName;
            Products = products ?? new List<Product>();
            ProductsCount = Products.Count();
            SetCheckedForInterselectedSizes(sizeSkusInSelection);
        }

        [DataMember(Name = "selectionName")] public string SelectionName { get; set; }

        [DataMember(Name = "products")] public IEnumerable<Product> Products { get; set; }
        public int ProductsCount { get; set; }

        //TODO ���������, ���� �� �������
        private void SetCheckedForInterselectedSizes(ICollection<string> sizeSkusInSelection)
        {
            // foreach (var productFromPIM in Products)
            // {
            //     var productFromPimSizes = productFromPIM.ColorSizes.SelectMany(x => x.Sizes);
            //
            //     foreach (var size in productFromPimSizes)
            //     {
            //         size.Checked = sizeSkusInSelection.Contains(size.Sku);
            //     }
            // }
        }
    }
}