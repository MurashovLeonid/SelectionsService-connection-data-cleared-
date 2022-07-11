using MediatR;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Superbrands.Selection.Domain.Events;

namespace Superbrands.Selection.Application.Procurement
{
    class UpdateProcurementQueryHandler : IRequestHandler<UpdateProcurementQuery, Domain.Procurements.Procurement>
    {
        private readonly IProcurementRepository _repository;

        public UpdateProcurementQueryHandler(IProcurementRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Domain.Procurements.Procurement> Handle(UpdateProcurementQuery request, CancellationToken cancellationToken)
        {
            var procurementDal = await _repository.GetById(request.Procurement.Id, cancellationToken);
            if (procurementDal == null)
                throw new Exception($"Procurement with id {request.Procurement.Id} not found");

            var procurement = procurementDal.ToDomain<ProcurementUpdatedEvent, Domain.Procurements.Procurement>();
            procurement.Edit(request.Procurement);
            var procurementDalDto = ProcurementDalDto.FromDomain(procurement);

            if (procurementDalDto.EntityModificationInfo == null) //todo убрать после фикса мапинге в либе
                procurementDalDto.EntityModificationInfo = procurementDal.EntityModificationInfo;
            
            
            foreach (var selectionMapped in procurementDalDto.Selections)
            {
                var originalSelection = procurementDal.Selections.FirstOrDefault(s => s.Id == selectionMapped.Id);
                foreach (var salePointKey in selectionMapped.SelectionPurchaseSalePointKeys)
                {
                    if (salePointKey.EntityModificationInfo == null)
                        salePointKey.EntityModificationInfo = originalSelection?.SelectionPurchaseSalePointKeys
                            ?.FirstOrDefault(x => x.Id == salePointKey.Id)?.EntityModificationInfo;
                }
                if (selectionMapped.EntityModificationInfo == null)
                {
                    selectionMapped.EntityModificationInfo = originalSelection?.EntityModificationInfo;
                }
                foreach (var modelMetaDalDto in selectionMapped.ColorModelMetas)
                {
                    if (modelMetaDalDto.EntityModificationInfo == null)
                    {
                            
                        modelMetaDalDto.EntityModificationInfo = originalSelection?.ColorModelMetas?
                            .FirstOrDefault(c => c.Id == modelMetaDalDto.Id)?.EntityModificationInfo;
                    }
                }
            }
            
            
            await _repository.Update(procurementDalDto, cancellationToken);
            await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return procurement;
        }
    }
}