using System;
using System.Collections.Generic;

namespace Superbrands.Selection.Domain
{
    public class KeyParameters
    {
        public long PartnerId { get; set; }
        public long SeasonCapsuleId { get; set; }
        public IEnumerable<long> MemberIds { get; set; }
    }
}
