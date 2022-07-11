using Superbrands.Selection.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Application.Responses
{
    public class ProductStatusCount
    {
        public ColorModelStatus ColorModelStatus { get; private set; }
        public int Count { get; private set; }

        public ProductStatusCount(ColorModelStatus colorModelStatus, int count)
        {
            if (count < 0)
                throw new ArgumentException(nameof(count));

            ColorModelStatus = colorModelStatus;
            Count = count;
        }
    }
}
