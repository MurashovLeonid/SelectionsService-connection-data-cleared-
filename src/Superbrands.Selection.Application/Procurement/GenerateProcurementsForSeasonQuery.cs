using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Superbrands.Libs.DDD.Abstractions;

namespace Superbrands.Selection.Application.Procurement;

public class GenerateProcurementsForSeasonQuery : IRequest<List<Domain.Procurements.Procurement>>
{
    public long SeasonCapsulesId { get;}
    public OperationLog Creator { get; }
        
    public GenerateProcurementsForSeasonQuery([NotNull] OperationLog creator, long seasonCapsulesId)
    {
        Creator = creator ?? throw new ArgumentNullException(nameof(creator));
        SeasonCapsulesId = seasonCapsulesId;
    }
}