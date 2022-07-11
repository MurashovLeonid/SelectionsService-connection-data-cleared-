using System;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain.Procurements;

namespace Superbrands.Selection.Domain.Events
{
    public class ProcurementDeletedEvent : IDomainRootReferenceEvent<Procurement>, INonDuplicatableDomainEventOfType<Procurement>
    {
        public Procurement CurrentState { get; set; }

        public ProcurementDeletedEvent(Procurement procurement)
        {
            CurrentState = procurement ?? throw new ArgumentNullException(nameof(procurement));
        }
    }
}