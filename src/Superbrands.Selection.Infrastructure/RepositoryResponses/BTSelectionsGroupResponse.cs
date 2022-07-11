using System.Collections.Generic;

namespace Superbrands.Selection.Infrastructure.RepositoryResponses
{
    public class BTSelectionsGroupResponse
    {
        public IEnumerable<PurchaseKeySalePointDto> PurchaseKeyDepartment { get; set; }
        public IEnumerable<long> MemberIds { get; set; }
    }
}
