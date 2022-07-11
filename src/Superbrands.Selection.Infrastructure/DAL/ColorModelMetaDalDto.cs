using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Domain.Enums;
using System.Collections.Generic;
using Mapster;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.Infrastructure.DAL
{
    public class ColorModelMetaDalDto : AuditableEntity
    {
        public string ModelVendorCodeSbs { get; set; }
        public long SelectionId { get; set; }
        public virtual SelectionDalDto Selection { get; set; }
        public string ColorModelVendorCodeSbs { get; set; }
        public List<Size> Sizes { get; set; }
        public int SizeChartId { get; set; }
        public ColorModelStatus ColorModelStatus { get; set; }
        public ColorModelPriority ColorModelPriority { get; set; }
        public int SizeChartCount { get; set; }
        public string Currency { get; set; }
        public virtual ColorModelGroupKeys ColorModelGroupKeys { get; set; }

        public ColorModelMeta ToDomain()
        {
            return new(ModelVendorCodeSbs, SelectionId, ColorModelVendorCodeSbs, ColorModelStatus,
                    ColorModelPriority, Sizes, Currency)
                {Id = Id, ColorModelGroupKeys = ColorModelGroupKeys, SizeChartCount = SizeChartCount, SizeChartId = SizeChartId};
        }

        public static ColorModelMetaDalDto FromDomain(ColorModelMeta colorModelMeta)
        {
            return colorModelMeta.Adapt<ColorModelMetaDalDto>();
        }
    }
}