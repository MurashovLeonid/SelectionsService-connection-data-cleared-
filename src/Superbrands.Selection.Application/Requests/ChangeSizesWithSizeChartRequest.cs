using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Application.Requests
{
    public class ChangeSizesWithSizeChartRequest
    {
        public string ColorModelVendorCodeSbs { get; set; }
        public int SizeChartId { get; set; }
        public int SizeChartCount { get; set; }
        public IEnumerable<SizeSkuAndCount> SizesSkuAndCount { get; set; }
        public IEnumerable<long> SalePointIds { get; set; }
    }

    public class SizeSkuAndCount
    {
        public string SizeSku { get; set; }
        public int SizeCount { get; set; }
    }
}
