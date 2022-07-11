using System.Collections.Generic;
using MediatR;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Application.Procurement
{
    public class SearchProcurementQuery : PagedQuery<ProcurementDalDto>
    {
        public List<long> PartnersIds { get; }
        public List<long> SeasonCapsulesIds { get; }
        public List<long> SalePointsIds { get; }
        public List<long> BuyersIds { get; }

        public SearchProcurementQuery(List<long> partnersIds, List<long> seasonCapsulesIds, List<long> salePointsIds, List<long> buyersIds, int? page,
            int? size) : base(page, size)
        {
            PartnersIds = partnersIds;
            SeasonCapsulesIds = seasonCapsulesIds;
            SalePointsIds = salePointsIds;
            BuyersIds = buyersIds;
        }
    }
}