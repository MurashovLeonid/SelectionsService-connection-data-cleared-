using JetBrains.Annotations;
using MediatR;
using Superbrands.Bus.Contracts.CSharp.MsPartnersInfrastructure.SalePoint;
using Superbrands.Libs.DDD.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Application.Procurement
{
    public class GenerateProcurementsForSalePointsQuery : IRequest<List<Domain.Procurements.Procurement>>
    {
        public List<long> SalePointIds { get; set; }           
        public OperationLog Creator { get; }

        public GenerateProcurementsForSalePointsQuery([NotNull] OperationLog creator, List<long> salePointIds)
        {
            Creator = creator ?? throw new ArgumentNullException(nameof(creator));
            SalePointIds = salePointIds;
        }
    }
}
