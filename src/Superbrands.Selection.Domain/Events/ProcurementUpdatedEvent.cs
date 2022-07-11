using System;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain.Procurements;

namespace Superbrands.Selection.Domain.Events
{
    public class ProcurementUpdatedEvent : IUpdateDomainEvent<Procurement>
    {

        /// <inheritdoc />
        public Procurement CurrentState { get; set; }
        
        /// <inheritdoc />
        public Procurement OriginalState { get; set; }
        
        public ProcurementUpdatedEvent(Procurement currentState, Procurement originalState)
        {
            OriginalState = originalState ?? throw new ArgumentNullException(nameof(originalState));
            CurrentState = currentState ?? throw new ArgumentNullException(nameof(currentState));
        }

      
    }
}