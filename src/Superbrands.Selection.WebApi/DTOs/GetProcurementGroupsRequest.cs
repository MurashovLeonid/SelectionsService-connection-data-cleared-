using Superbrands.Selection.Domain.Enums;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class GetProcurementGroupsRequest
    {
        public ProcurementsFiltersDto ProcurementsFiltersDto { get; set; }
        public ProcurementSort procurementSort { get; set;  }
        public ProcurementGroupTypeEnum ProcurementGroupType { get; set; }
        public byte HierarchyShift { get; set; }
        public bool SortByAsc { get; set; }
    }
}
