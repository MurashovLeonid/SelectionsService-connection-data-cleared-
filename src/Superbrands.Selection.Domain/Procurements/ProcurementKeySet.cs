using System;
using FastDeepCloner;
using Superbrands.Libs.DDD.Abstractions;

namespace Superbrands.Selection.Domain.Procurements
{
    public class ProcurementKeySet : EntityBase
    {
        public long ProcurementId { get; protected set; }
        public int BuyerId { get; protected set; }
        public int PurchaseKeyId { get; protected set; }
        public int FinancialPlaningCenterId { get; protected set; }
        [NoneCloneable]
        public virtual Procurement Procurement { get; set; }

        protected ProcurementKeySet() 
        {
        }

        public ProcurementKeySet(long procurementId, int buyerId, int purchaseKeyId, int financialPlaningCenterId)
        {
            if (procurementId <= 0)
                throw new ArgumentException(nameof(procurementId));

            if (buyerId <= 0)
                throw new ArgumentException(nameof(buyerId));

            if (purchaseKeyId <= 0)
                throw new ArgumentException(nameof(purchaseKeyId));

            if (financialPlaningCenterId <= 0)
                throw new ArgumentException(nameof(financialPlaningCenterId));

            ProcurementId = procurementId;
            BuyerId = buyerId;
            PurchaseKeyId = purchaseKeyId;
            FinancialPlaningCenterId = financialPlaningCenterId;
        }
    }
}
