using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Superbrands.Libs.DDD.Abstractions;

namespace Superbrands.Selection.Application.Procurement
{
    public class UpdateProcurementQuery : IRequest<Domain.Procurements.Procurement>
    {
        public Domain.Procurements.Procurement Procurement { get; }
        public OperationLog Updater { get; }


        public UpdateProcurementQuery(Domain.Procurements.Procurement procurement, [NotNull] OperationLog updater)
        {
            Procurement = procurement ?? throw new ArgumentNullException(nameof(procurement));
            Updater = updater ?? throw new ArgumentNullException(nameof(updater));
        }
    }
}
