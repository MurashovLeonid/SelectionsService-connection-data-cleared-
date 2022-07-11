using System.Collections.Generic;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class GroupKeyParametersDto
    {
        public int PartnerId { get; set; }
        public int SeasonId { get; set; }
        public int SeasonCapsuleId { get; set; }
        public IEnumerable<int> MemberIds { get; set; }
    }
}
