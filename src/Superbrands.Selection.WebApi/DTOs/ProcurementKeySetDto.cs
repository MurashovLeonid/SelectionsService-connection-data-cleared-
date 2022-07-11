using System.Collections.Generic;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class ProcurementKeySetDto
    {
        public long ProcurementId { get; set; }
        public int BuyerId { get; set; }
        public int PurchaseKeyId { get; set; }
        public int FinancialPlaningCenterId { get; set; }
        public List<long> ManagerIds { get; set; }
    }
}
