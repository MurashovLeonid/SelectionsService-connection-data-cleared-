using MediatR;
using Superbrands.Selection.Domain.Events;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Superbrands.Selection.Application.Selection.DomainEvents
{
    internal class AddedSelectionPurchaseSalePointKeyToSelectionDomainEventHandler : INotificationHandler<AddedSelectionPurchaseSalePointKeyToSelectionDomainEvent>
    {
        private readonly ISelectionRepository _selectionRepository;

        public AddedSelectionPurchaseSalePointKeyToSelectionDomainEventHandler(ISelectionRepository selectionRepository)
        {
            _selectionRepository = selectionRepository ??
                                        throw new ArgumentNullException(nameof(selectionRepository));
        }

        public async Task Handle(AddedSelectionPurchaseSalePointKeyToSelectionDomainEvent notification, CancellationToken cancellationToken)
        {
            var dalDtos = notification.Selection.SelectionPurchaseSalePointKeys.Select(SelectionPurchaseSalePointKeyDalDto.FromDomain);
            var dalDtosWithSelectionId = dalDtos.Select(x => { x.SelectionId = notification.Selection.Id; return x; });

            await _selectionRepository.AddSelectionPurchaseSalePointKeys(dalDtosWithSelectionId);
            await _selectionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
