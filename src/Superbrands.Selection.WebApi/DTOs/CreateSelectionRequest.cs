using System.Runtime.Serialization;

namespace Superbrands.Selection.WebApi.DTOs
{
    [DataContract]
    public class CreateSelectionRequest
    {
        [DataMember(Name = "procurementId")]
        public long ProcurementId { get; set; }
    }
}