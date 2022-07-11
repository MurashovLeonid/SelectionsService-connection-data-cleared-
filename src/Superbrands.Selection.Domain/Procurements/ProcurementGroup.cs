using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.Domain.Procurements
{
    public class ProcurementGroup
    {
        public int ProcurementId { get; set; }
        public KeyParameters KeyParameters { get; set; }
        public ProductGroupMeta Meta { get; set; }
    }
}
