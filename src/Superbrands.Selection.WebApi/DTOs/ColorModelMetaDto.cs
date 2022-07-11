using Superbrands.Selection.Domain.Enums;
using System.Collections.Generic;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class ColorModelMetaDto
    {
        public ColorModelMetaDto()
        {

        }

        public ColorModelMetaDto(ColorModelMeta colorModelMeta)
        {
            ModelVendorCodeSbs = colorModelMeta.ModelVendorCodeSbs;
            SelectionId = colorModelMeta.SelectionId;
            ColorModelVendorCodeSbs = colorModelMeta.ColorModelVendorCodeSbs;
            Sizes = colorModelMeta.Sizes;
            SizeChartId = colorModelMeta.SizeChartId;
            ColorModelStatus = colorModelMeta.ColorModelStatus;
            ColorModelPriority = colorModelMeta.ColorModelPriority;
            SizeChartCount = colorModelMeta.SizeChartCount;
            ColorModelGroupKeys = colorModelMeta.ColorModelGroupKeys;
            Currency = colorModelMeta.Currency;
        }

        public string ModelVendorCodeSbs { get;  set; }

        public long SelectionId { get;  set; }

        public string ColorModelVendorCodeSbs { get;  set; }

        public List<Size> Sizes { get; set; }

        public int SizeChartId { get; set; }

        public ColorModelStatus ColorModelStatus { get; protected set; }

        public ColorModelPriority ColorModelPriority { get; protected set; }

        public int SizeChartCount { get; set; }

        public ColorModelGroupKeys ColorModelGroupKeys { get; set; }
        
        public string Currency { get; set; }
    }
}
