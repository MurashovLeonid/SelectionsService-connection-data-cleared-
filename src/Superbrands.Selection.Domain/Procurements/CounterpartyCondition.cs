using System;
using System.Collections.Generic;
using Superbrands.Libs.DDD.Abstractions;

namespace Superbrands.Selection.Domain.Procurements
{
    public class CounterpartyCondition : ValueObject
    {
        public CounterpartyCondition(long counterpartyId, long relationshipId, long agreementId)
        {
            if (counterpartyId < 0)
                throw new ArgumentException($"{nameof(counterpartyId)} id must be greater than 0");
            if (relationshipId < 0)
                throw new ArgumentException($"{nameof(relationshipId)} id must be greater than 0");
            if (agreementId < 0)
                throw new ArgumentException($"{nameof(agreementId)} id must be greater than 0");
            CounterpartyId = counterpartyId;
            RelationshipId = relationshipId;
            AgreementId = agreementId;
        }

        public long CounterpartyId { get; }
        public long RelationshipId { get; }
        public long AgreementId { get; }
        
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return CounterpartyId;
            yield return RelationshipId;
            yield return AgreementId;
        }
    }
}