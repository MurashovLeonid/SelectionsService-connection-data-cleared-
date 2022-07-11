using MediatR;
using Superbrands.Selection.Application.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Superbrands.Selection.Application.Selection
{
    public class ChangeSizesWithSizeChartQuery : IRequest<Unit>
    {
        public string ColorModelVendorCodeSbs { get; }
        public int SizeChartId { get; }
        public int SizeChartCount { get; }
        public IEnumerable<SizeSkuAndCount> SizesSkuAndCount { get; }
        public IEnumerable<long> SalePointIds { get; }

        public ChangeSizesWithSizeChartQuery(string colorModelVendorCodeSbs, int sizeChartId, int sizeChartCount,
            IEnumerable<SizeSkuAndCount> sizesSkuAndCount, IEnumerable<long> salePointIds)
        {
            if (!sizesSkuAndCount?.Any() ?? true) throw new ArgumentNullException(nameof(sizesSkuAndCount));
            if (sizeChartId <= 0) throw new ArgumentOutOfRangeException(nameof(sizeChartId));
            if (sizeChartCount <= 0) throw new ArgumentOutOfRangeException(nameof(sizeChartCount));
            if (string.IsNullOrWhiteSpace(colorModelVendorCodeSbs))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(colorModelVendorCodeSbs));

            ColorModelVendorCodeSbs = colorModelVendorCodeSbs;
            SalePointIds = salePointIds ?? throw new ArgumentNullException(nameof(salePointIds));
            SizeChartId = sizeChartId;
            SizeChartCount = sizeChartCount;
            SizesSkuAndCount = sizesSkuAndCount;
        }
    }
}