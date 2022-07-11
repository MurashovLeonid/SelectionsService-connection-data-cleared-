using Superbrands.Selection.Domain.Requests;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Superbrands.Selection.Application;
using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class AddProductToSelectionRequest
    {
        public List<ModelWithColors> Products { get; set; }
        public IEnumerable<long> SalePointIds { get; set; }
        public long ProcurementId { get; set; }
    }


    [DataContract]
    public class AddColorModelToProductRequest
    {
        [DataMember(Name = "b2bProductId")]
        public long B2BProductId { get; set; }

        [DataMember(Name = "selectionId")]
        public long SelectionId { get; set; }

        [DataMember(Name = "colorModel")]
        public ColorModelMeta ColorModel { get; set; }
    }

    [DataContract]
    public class ChangeProductsQuantity
    {
        [DataMember(Name = "salePointId")]
        public long SalePointId { get; set; }

        [DataMember(Name = "colorModelVendorCodeSbs")]
        public string ColorModelVendorCodeSbs { get; set; }

        [DataMember(Name = "sizeChartCount")]
        public int SizeChartCount { get; set; }

        [DataMember(Name = "sizeChartId")]
        public int SizeChartId { get; set; }

        [DataMember(Name = "SizeInfos")]
        public IEnumerable<SizeInfo> SizeInfos {get; set;}
    }
}