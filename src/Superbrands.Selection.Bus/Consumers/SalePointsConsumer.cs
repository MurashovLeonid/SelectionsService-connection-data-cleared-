using EasyNetQ.AutoSubscribe;
using MediatR;
using Microsoft.Extensions.Logging;
using Superbrands.Bus.Contracts.CSharp;
using Superbrands.Bus.Contracts.CSharp.MsPartnersInfrastructure.SalePoint;
using Superbrands.Selection.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Superbrands.Selection.Application.Procurement;





namespace Superbrands.Selection.Bus.Consumers
{
    internal class SalePointsConsumer : IConsumeAsync<BusMessages<SalePoint>>
    {      
        private readonly ILogger<SalePointsConsumer> _logger;
        private readonly IMediator _mediator;
              
        public SalePointsConsumer( ILogger<SalePointsConsumer> logger, IMediator mediator)
        {          
            _logger = logger;
            _mediator = mediator;                    
        }
        public async Task ConsumeAsync(BusMessages<SalePoint> message, CancellationToken cancellationToken = default)
        {                     
            var salePointIdsList = message.Messages
                                        .Where(x => (x.EventType == CrudEventType.Create && x.NewState.StatusInfo.Status == SalePointStatus.Open) || (x.EventType == CrudEventType.Update && x.NewState.StatusInfo.Status == SalePointStatus.Open))?
                                        .Select(x => x.NewState.Id)?
                                        .ToList() ?? null;
           
            if (salePointIdsList!= null && salePointIdsList.Any())
            {
                try
                {
                    var query = new GenerateProcurementsForSalePointsQuery(new Libs.DDD.Abstractions.OperationLog("Consumer", "1", DateTime.Now), salePointIdsList);
                    await _mediator.Send(query, cancellationToken);                 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }

            var deletedSalePointsList = message.Messages
                                              .Where(q => q.EventType == CrudEventType.Update && q.NewState.Status != SalePointStatus.Open)?
                                              .Select(x => x.NewState.Id)?
                                              .ToList() ?? null; // Получаем список ТП, со статусом отличным от Open, данные ТП теперь будут считаться закрытыми, и их необходимо удалить из закупки

            if (deletedSalePointsList != null && deletedSalePointsList.Any())
            {
                try
                {
                    var query = new DeleteSalePointsFromProcurementQuery(deletedSalePointsList);
                    await _mediator.Send(query, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }     
    }
}
