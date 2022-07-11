using MediatR;
using Superbrands.Selection.Application.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Application.Procurement
{
    public class CreateProcurementForPartnerQuery : IRequest<IEnumerable<long>>
    {
        public IEnumerable<CreateProcurementForPartnerRequest> CreateProcurementForPartnerRequests { get; }
        public int OperatorId { get; }

        public CreateProcurementForPartnerQuery(
            IEnumerable<CreateProcurementForPartnerRequest> createProcurementForPartnerRequests, int operatorId)
        {
            if (createProcurementForPartnerRequests == null)
                throw new ArgumentNullException(nameof(createProcurementForPartnerRequests));

            foreach (var createProcurementForPartnerRequest in createProcurementForPartnerRequests)
            {
                if (createProcurementForPartnerRequest.PartnerId <= 0)
                    throw new ArgumentException("PartnerId should be set");

                if (createProcurementForPartnerRequest.SeasonCapsuleId == 0)
                    throw new ArgumentException(
                        $"{nameof(createProcurementForPartnerRequest.SeasonCapsuleId)} should be set");
            }

            if (operatorId < 0)
                throw new ArgumentException($"{nameof(operatorId)} should be set");

            OperatorId = operatorId;
            CreateProcurementForPartnerRequests = createProcurementForPartnerRequests;
        }
    }
}