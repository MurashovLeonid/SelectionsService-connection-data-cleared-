using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;

namespace Superbrands.Selection.Application.Selection
{
    public class GetSelectionsBySalePointIdsQuery : IRequest<List<Domain.Selections.Selection>>
    {
        public GetSelectionsBySalePointIdsQuery([NotNull] List<long> salePointsIds)
        {
            SalePointsIds = salePointsIds ?? throw new ArgumentNullException(nameof(salePointsIds));
        }

        public List<long> SalePointsIds { get; }
    }
}