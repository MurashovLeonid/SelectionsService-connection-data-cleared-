using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MediatR;
using Superbrands.Selection.Domain.Procurements;

namespace Superbrands.Selection.Application.Procurement
{
    public class CreateCounterpartyConditionQuery : IRequest<List<CounterpartyCondition>>
    {
        public CreateCounterpartyConditionQuery(long partnerId, [NotNull] IEnumerable<long> counterpartiesIds)
        {
            if (partnerId < 0)
                throw new ArgumentException($"{nameof(partnerId)} id must be greater than 0");
            if (counterpartiesIds == null || !counterpartiesIds.Any())
                throw new ArgumentException($"{nameof(counterpartiesIds)} can't be empty");
            PartnerId = partnerId;
            CounterpartiesIds = counterpartiesIds;
        }

        public long PartnerId { get; }
        public IEnumerable<long> CounterpartiesIds { get; }
    }
}