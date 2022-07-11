using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastDeepCloner;
using JetBrains.Annotations;
using MediatR;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain.Events;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Application.Selection
{
    internal class UpsertSelectionQueryHandler : IRequestHandler<UpsertSelectionQuery, Domain.Selections.Selection>
    {
        private readonly ISelectionRepository _selectionsRepository;
        private readonly IProcurementRepository _procurementRepository;
        private readonly IMediator _mediator;

        public UpsertSelectionQueryHandler(ISelectionRepository repository,
            [NotNull] IProcurementRepository procurementRepository, [NotNull] IMediator mediator)
        {
            _selectionsRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _procurementRepository = procurementRepository ?? throw new ArgumentNullException(nameof(procurementRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Domain.Selections.Selection> Handle(UpsertSelectionQuery request, CancellationToken cancellationToken)
        {
            var originalProcurement = await _procurementRepository.GetById(request.Selection.ProcurementId, cancellationToken);
            var selectionDalDto = SelectionDalDto.FromDomain(request.Selection);
            _selectionsRepository.RemoveProducts(request.Selection.ColorModelMetas.Where(c => c.Removed)
                .Select(ColorModelMetaDalDto.FromDomain).ToList());
            await _selectionsRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            var original = (await _selectionsRepository.GetSelectionsByIds(new[] {selectionDalDto.Id}, cancellationToken))
                .FirstOrDefault();
            

            if (original != default)
            {
                if (selectionDalDto.EntityModificationInfo == null)
                    selectionDalDto.EntityModificationInfo = original.EntityModificationInfo;

                foreach (var modelMetaDalDto in selectionDalDto.ColorModelMetas)
                    if (modelMetaDalDto.EntityModificationInfo == null)
                        modelMetaDalDto.EntityModificationInfo = original.EntityModificationInfo;
            }

            if (selectionDalDto.Id == 0)
                await _selectionsRepository.Add(selectionDalDto, cancellationToken);
            else
                await _selectionsRepository.Update(selectionDalDto, cancellationToken);

            await _selectionsRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _selectionsRepository.Detach(selectionDalDto);
            if (request.Selection.ProcurementId >= 0)
            {
                var procurement = await _procurementRepository.GetById(request.Selection.ProcurementId, cancellationToken);
                var procurementDomain = procurement.ToDomain<ProcurementUpdatedEvent, Domain.Procurements.Procurement>();
                var domainEvent = new ProcurementUpdatedEvent(procurementDomain, originalProcurement.ToDomain<ProcurementUpdatedEvent, Domain.Procurements.Procurement>());

                await _mediator.Publish(new DomainEventsOfType<ProcurementUpdatedEvent>(new[] {domainEvent}), cancellationToken);
                if (procurementDomain.IsApproved())
                {
                    procurement.AddEvent(
                        new ProcurementApprovedEvent(procurement
                            .ToDomain<ProcurementUpdatedEvent, Domain.Procurements.Procurement>()));
                    await _procurementRepository.Update(procurement, cancellationToken);
                }

                if (procurementDomain.Selections.Count != originalProcurement.Selections.Count)
                {
                    procurement.AddEvent(new ProcurementSelectionAddedEvent(procurementDomain, 
                        originalProcurement.ToDomain<ProcurementUpdatedEvent, Domain.Procurements.Procurement>()));
                    await _procurementRepository.Update(procurement, cancellationToken); 
                }


                var selection = procurement.Selections.FirstOrDefault(s => s.Id == request.Selection.Id);
                if (selection != null)
                {
                    var colorModelMetasToDelete = selection.ColorModelMetas
                        .Where(c => request.Selection.ColorModelMetas.All(s => s.Id != c.Id))
                        .ToList();
                    colorModelMetasToDelete.ForEach(c => c.Selection = selectionDalDto);
                    _selectionsRepository.RemoveProducts(colorModelMetasToDelete);
                }
            }

            await _selectionsRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
         

            return request.Selection;
        }
    }
}