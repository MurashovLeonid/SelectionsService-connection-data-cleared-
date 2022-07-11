using EasyNetQ.AutoSubscribe;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Superbrands.Bus.Contracts.CSharp;
using Superbrands.Bus.Contracts.CSharp.MsSeasons;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Application.Procurement;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Superbrands.Selection.Domain.Events;
using Superbrands.Selection.Domain.Procurements;

namespace Superbrands.Selection.Bus.Consumers
{
    internal class SeasonCapsuleConsumer : IConsumeAsync<BusMessages<SeasonCapsule>>
    {
        private readonly IProcurementRepository _procurementRepository;
        private readonly ILogger<SeasonCapsuleConsumer> _logger;
        private readonly IMediator _mediator;

        public SeasonCapsuleConsumer(IProcurementRepository procurementRepository, ILogger<SeasonCapsuleConsumer> logger, IMediator mediator)
        {
            _procurementRepository = procurementRepository ?? throw new ArgumentNullException(nameof(procurementRepository));
            _logger = logger;
            _mediator = mediator;
        }

        public async Task ConsumeAsync(BusMessages<SeasonCapsule> message, CancellationToken cancellationToken = default)
        {
            var updatedSeasonList = message.Messages.Where(q => q.EventType == CrudEventType.Update).ToList();

            if (updatedSeasonList.Any())
            {
                try
                {
                    var changedStatusSeasonList = updatedSeasonList.Where(q => q.NewState.Status != q.OriginalState.Status)
                        .Select(item => item.NewState);

                    var SeasonIdToStatus = changedStatusSeasonList.ToDictionary(i => i.Id, s => s.Status);
                    var procurementLists =
                        await _procurementRepository.GetBySeasonCapsuleIds(changedStatusSeasonList.Select(q => q.Id).ToList(), cancellationToken);

                    foreach (var procurement in procurementLists.ToLookup(x => x.SeasonId))
                    {
                        if (!SeasonIdToStatus.TryGetValue(procurement.Key, out var seasonCapsuleStatus))
                            continue;

                        var produrements = procurement.Select(q => q.ToDomain<Procurement>()).ToList();

                        produrements.ForEach(q => q.EditSeasonCapsuleStatus(seasonCapsuleStatus == SeasonCapsuleStatus.PreOrder));

                        _procurementRepository.EditRange(produrements.Select(ProcurementDalDto.FromDomain).ToList());
                    }

                    await _procurementRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
            var createdSeasonMessagesList = message.Messages.Where(q => q.EventType == CrudEventType.Create).ToList();
            if(createdSeasonMessagesList.Any())
            {
                try
                {
                    var createdSeasonList = createdSeasonMessagesList.Select(x => x.NewState)?.ToList();
                    foreach (var createdSeason in createdSeasonList)
                    {
                        var operationLog = new OperationLog("SeasonsConsumer", "1", DateTime.Now);
                        var query = new GenerateProcurementsForSeasonQuery(operationLog, createdSeason.Id);
                        var procurements = await _mediator.Send(query, cancellationToken);
                        
                    }
                                            
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                
            }
        }
    }
}