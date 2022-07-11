using System.Collections.Generic;
using System.Linq;
using Superbrands.Libs.RestClients.Pim;
using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.Infrastructure.Helpers
{
    public static class SelectionHelper
    {
        public static void FilterOutProductsNotInSelection(IEnumerable<ColorModelMeta> productsMeta, IEnumerable<ProductData> pimProducts)
        {
            var sizesInSelection = new HashSet<string>(productsMeta.SelectMany(s => s.Sizes).Select(s => s.Sku).ToList());

            foreach (var pimProductDto in pimProducts)
            {
                foreach (var colorLevelDto in pimProductDto.ColorLevel)
                {
                    colorLevelDto.RangeSizeLevel.Select(x => x.IsChecked = false).ToList();
                    colorLevelDto.RangeSizeLevel.Where(c => sizesInSelection.Contains(c.Sku))
                        .Select(x => x.IsChecked = true).ToList();
                }
            }
        }
    }
}