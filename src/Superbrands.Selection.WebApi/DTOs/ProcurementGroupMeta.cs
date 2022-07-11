namespace Superbrands.Selection.WebApi.DTOs
{
    public class ProcurementGroupMetaDto
    {
        public int ProductsCount { get; set; }
        public double AverageRrc { get; set; }
        public double AverageBwp { get; set; }
        public double Coefficient { get; set; }
        public double Marginality { get; set; }
        public int ColorModelCount { get; set; }
        public int SizesCount { get; set; }
    }
}
