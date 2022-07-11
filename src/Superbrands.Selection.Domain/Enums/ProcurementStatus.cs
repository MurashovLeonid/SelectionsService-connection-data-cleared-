using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Domain.Enums
{
    public enum ProcurementStatus
    {
        None = 0,
        ReadyForSelection = 1,
        Selection = 2,
        OnApproval = 3,
        OnCompletion = 4,
        Analysis = 5,
        OnSigning = 6,
        OrdersSigned = 7,
        AdvancesPaid = 8,
        AtTheApprovalOfReplacements = 9,
        Execution = 10,
        OrdersShipped = 11,
        OrdersPaid = 12,
        GoodsDelivered = 13,
        ArchiveCompleted = 14,
        ArchiveNotImplemented = 15,
        ArchiveTerminated = 16
    }
}
