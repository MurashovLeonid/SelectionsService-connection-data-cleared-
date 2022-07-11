using System.Collections.Generic;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class SearchProcurementRequest
    {
        public int? Page { get; set; }
        public int? Size { get; set; }

        public List<long> PartnersIds { get; set; }

        public List<long> SeasonCapsulesIds { get; set; }

        public List<long> SalePointsIds { get; set; }

        public List<long> BuyersIds { get; set; }
    }
}