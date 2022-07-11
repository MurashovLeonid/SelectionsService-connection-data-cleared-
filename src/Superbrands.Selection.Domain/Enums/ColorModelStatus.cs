using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Domain.Enums
{
    [Flags]
    public enum ColorModelStatus
    {
        None = 0,
        New = 2,
        Canceled = 4,
        PriceChanged = 8,
        QuantityChanged = 16,
        Archive = 32
    }
}
