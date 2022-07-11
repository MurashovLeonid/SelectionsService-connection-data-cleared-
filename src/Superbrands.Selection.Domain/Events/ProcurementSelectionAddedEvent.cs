using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain.Procurements;

namespace Superbrands.Selection.Domain.Events
{
    public class ProcurementSelectionAddedEvent :  IDomainRootReferenceEvent<Procurement>, INonDuplicatableDomainEventOfType<Procurement>
    {
        public Procurement CurrentState { get; set; }
        public Procurement OriginalState { get; set; }

        public ProcurementSelectionAddedEvent(Procurement currentState, Procurement originalState)
        {
            CurrentState = currentState;
            OriginalState = originalState;
        }
    }
}