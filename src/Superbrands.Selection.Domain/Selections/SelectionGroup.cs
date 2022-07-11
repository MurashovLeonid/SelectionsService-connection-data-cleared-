namespace Superbrands.Selection.Domain.Selections
{
    public class SelectionGroup
    {
        public long SelectionId { get; set; }
        public KeyParameters KeyParameters { get; set; }
        public ProductGroupMeta Meta { get; set; }
    }
}
