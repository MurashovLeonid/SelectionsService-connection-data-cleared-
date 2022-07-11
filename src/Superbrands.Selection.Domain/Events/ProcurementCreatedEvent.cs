using System;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain.Procurements;

namespace Superbrands.Selection.Domain.Events
{
    public class ProcurementCreatedEvent : IDomainRootReferenceEvent<Procurement>, INonDuplicatableDomainEventOfType<Procurement>
    {
        public Procurement CurrentState { get; set; }

        public ProcurementCreatedEvent(Procurement procurement)
        {
            CurrentState = procurement ?? throw new ArgumentNullException(nameof(procurement));
        }
    }
}