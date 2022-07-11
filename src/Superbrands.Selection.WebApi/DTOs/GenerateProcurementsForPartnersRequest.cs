using System.Collections.Generic;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class GenerateProcurementsForPartnersRequest
    {
        public List<long> PartnersIds { get; set; }
        public List<long> SeasonCapsulesIds { get; set; }
    }
}