using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Domain.Enums
{
    public enum SelectionStatus : long
    {
        Unset = 0,
        InProgress = 1,
        OnApproval = 2,
        Agreed = 3,
        SelectionIsEmpty = 4
    }
}
