using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Superbrands.Libs.HttpClient;
using Superbrands.Libs.RestClients.Partners;
using Superbrands.Selection.Domain.Procurements;

namespace Superbrands.Selection.Application.Procurement
{
    internal class
        CreateCounterpartyConditionQueryHandler : IRequestHandler<CreateCounterpartyConditionQuery,
            List<CounterpartyCondition>>
    {
        private readonly IPartnersRelationsClient _relationsClient;
        

        public CreateCounterpartyConditionQueryHandler([NotNull] IPartnersRelationsClient relationsClient)
        {
            _relationsClient = relationsClient ?? throw new ArgumentNullException(nameof(relationsClient));
        }

        public async Task<List<CounterpartyCondition>> Handle(CreateCounterpartyConditionQuery request, CancellationToken cancellationToken)
        {
            _relationsClient.AuthAsMicroservice();
            var relationships =
                await _relationsClient.Relations_GetRelationshipsByPartnerAsync(request.PartnerId, null, cancellationToken);
            var result = new List<CounterpartyCondition>();

            foreach (var counterpartyId in request.CounterpartiesIds)
            {
                var selectedAgreement = relationships
                    .Where(c => c.CounterpartyId == counterpartyId)
                    .SelectMany(r => r.Agreements?.Select(a => new {Relationship = r, Agreement = a}))
                    .OrderByDescending(c => c.Agreement?.IsStandardForCounterparties)
                    .ThenBy(c => c.Agreement?.IsDefaultForRelationshipType).FirstOrDefault();

                if (selectedAgreement != null)
                    result.Add(new CounterpartyCondition(counterpartyId, selectedAgreement.Relationship.Id, selectedAgreement.Agreement.Id));
            }


            return result;
        }
    }
}