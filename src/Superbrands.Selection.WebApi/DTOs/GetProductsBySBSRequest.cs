using System.Collections.Generic;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class GetProductsBySBSRequest
    {
        public List<string> ModelVendoreCodeSbs { get; set; }
        public long? SelectionId { get; set; }
        public long? ProcurementId { get; set; }
        public long? SalePointId { get; set; }
        public long? PurchaseKeyId { get; set; }
    }
}