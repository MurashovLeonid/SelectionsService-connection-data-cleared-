namespace Superbrands.Selection.WebApi.DTOs
{
    public class ProcurementGroupDto
    {
        public int ProcurementId { get; set; }
        public GroupKeyParametersDto ProcurementGroupKeyParameters { get; set; }
        public ProcurementGroupMetaDto ProcurementGroupMeta { get; set; }
    }
}
