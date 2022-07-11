using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Superbrands.Libs.DDD.Abstractions;

namespace Superbrands.Selection.Application.Procurement
{
    public class GenerateProcurementsForPartnersQuery : IRequest<List<Domain.Procurements.Procurement>>
    {
        public List<long> PartnersIds { get;}
        public List<long> SeasonCapsulesIds { get;}
        public OperationLog Creator { get; }

        public GenerateProcurementsForPartnersQuery(List<long> partnersIds, List<long> seasonCapsulesIds, [NotNull] OperationLog creator)
        {
            Creator = creator ?? throw new ArgumentNullException(nameof(creator));
            PartnersIds = partnersIds ?? new List<long>();
            SeasonCapsulesIds = seasonCapsulesIds ?? new List<long>();
        }
    }
}