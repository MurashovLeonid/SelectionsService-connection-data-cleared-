using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Superbrands.Selection.WebApi.DTOs
{
    [DataContract]
    public class RemoveProductFromSelectionRequest
    {
        [DataMember(Name = "modelVendorCodeSbs"), Required(AllowEmptyStrings = false)]
        public string ModelVendorCodeSbs { get; set; }
    }

    [DataContract]
    public class RemoveColorModelFromSelectionRequest
    {
        [DataMember(Name = "departmentId")] public long SalePointId { get; set; }

        [DataMember(Name = "colorModelVendorCodeSbs", IsRequired = true)]
        public string ColorModelVendorCodeSbs { get; set; }

        [DataMember(Name = "b2bProductId")] public long ModelId { get; set; }
    }

    [DataContract]
    public class DeleteSizeFromProduct : RemoveProductFromSelectionRequest
    {
        [DataMember(Name = "sizeSku")] public string sizeSku { get; set; }
    }
}