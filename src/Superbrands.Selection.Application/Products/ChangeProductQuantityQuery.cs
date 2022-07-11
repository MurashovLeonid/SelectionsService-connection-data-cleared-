using System;
using System.Collections.Generic;
using MediatR;
using Superbrands.Bus.Contracts.CSharp.MsSelections.Selections;
using Superbrands.Selection.Domain.Requests;

namespace Superbrands.Selection.Application.Products
{
    public class ChangeProductQuantityQuery : IRequest
    {
        public int SizeChartCount { get; }
        public IEnumerable<SizeInfo> SizeInfos { get;  }
        public long ColorModelMetaId { get;  }
        public int SizeChartId { get; }

        public ChangeProductQuantityQuery(int sizeChartCount, IEnumerable<SizeInfo> sizeInfos, long colorModelMetaId, int sizeChartId)
        {
            if (colorModelMetaId <= 0)
                throw new ArgumentException(nameof(colorModelMetaId));

            if (sizeChartCount <= 0)
                throw new ArgumentException(nameof(sizeChartCount));

            SizeChartCount = sizeChartCount;
            SizeInfos = sizeInfos ??  throw new ArgumentNullException(nameof(sizeInfos));
            ColorModelMetaId = colorModelMetaId;
            SizeChartId = sizeChartId;
        }
    }

}