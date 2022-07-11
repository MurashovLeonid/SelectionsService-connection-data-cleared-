using System.Collections.Generic;

namespace Superbrands.Selection.Application.Responses
{
    public class ProductData : Superbrands.Libs.RestClients.Pim.ProductData
    {
        public new ICollection<ColorLevel> ColorLevel { get; set; }
    }

    public class ColorLevel : Superbrands.Libs.RestClients.Pim.ColorLevel
    {
        public int SizeChartId { get; set; }
        public int SizeChartCount { get; set; }
    }
}