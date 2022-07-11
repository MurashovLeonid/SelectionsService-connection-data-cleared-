using System;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain.Procurements;

namespace Superbrands.Selection.Domain.Events
{
    public class ProcurementApprovedEvent : INonDuplicatableDomainEventOfType<Procurement>
    {
        public Procurement Procurement { get; }

        public ProcurementApprovedEvent(Procurement procurement)
        {
            Procurement = procurement ?? throw new ArgumentNullException(nameof(procurement));
        }
    }
}