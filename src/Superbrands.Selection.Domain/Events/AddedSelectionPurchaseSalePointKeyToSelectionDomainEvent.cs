using Superbrands.Libs.DDD.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Domain.Events
{
    public class AddedSelectionPurchaseSalePointKeyToSelectionDomainEvent : INonDuplicatableDomainEventOfType<Selections.Selection>
    {
        public Selections.Selection Selection { get; }

        public AddedSelectionPurchaseSalePointKeyToSelectionDomainEvent(Selections.Selection selection)
        {
            Selection = selection ?? throw new ArgumentNullException(nameof(selection));
        }
    }
}
