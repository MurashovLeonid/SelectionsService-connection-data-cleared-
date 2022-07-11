using Superbrands.Selection.Domain.Enums;

namespace Superbrands.Selection.Domain.Selections
{
    public class SelectionGroupMeta
    {
        public int ProductsCount { get; set; }
        public double AverageRrc { get; set; }
        public double AverageBwp { get; set; }
        public double Marginality { get; set; }
        public int ColorModelCount { get; set; }
        public int SizesCount { get; set; }
        public GroupKeyType GroupKeyType { get; set; }
    }
}
