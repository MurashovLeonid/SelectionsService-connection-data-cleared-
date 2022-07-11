using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class ProductStatisticDto
    {
        public string ModelVendoreCodeSbs { get; set; }
        public IEnumerable<ColorModelVendoreCodesBySalePoints> ColorModelVendoreCodesBySalePoints { get; set; }
    }

    public class ColorModelVendoreCodesBySalePoints
    {
        public long SalePointId { get; set; }
        public List<string> ColorModelVendoreCodes { get; set; }
    }
}
