using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Superbrands.Bus.Contracts.CSharp;
using Superbrands.Bus.Contracts.CSharp.Orders;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Application;
using Superbrands.Selection.Application.Procurement;
using Superbrands.Selection.Application.Selection;

namespace Superbrands.Selection.Bus.Consumers
{
    public class OrdersConsumer : IConsumeAsync<BusMessages<Order>>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersConsumer> _logger;

        public OrdersConsumer(IMediator mediator, [NotNull] ILogger<OrdersConsumer> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ConsumeAsync(BusMessages<Order> messages, CancellationToken cancellationToken = new CancellationToken())
        {
            
            try
            {
                _logger.LogInformation($"Received message with order {messages?.Messages?.FirstOrDefault()?.NewState?.Id}");
                await ProcessCreated(messages, cancellationToken);
                await ProcessCancelled(messages, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in OrdersConsumer");
            }
        }

        private async Task ProcessCancelled(BusMessages<Order> messages, CancellationToken cancellationToken)
        {
            var cancelledOrders = messages.Messages.Where(m => m.EventType == CrudEventType.Deleted).ToList();

            foreach (var message in cancelledOrders)
            {
                var order = message.NewState;
                await _mediator.Send(
                    new AddProductsToSelectionQuery(new [] { order.SalePointId.GetValueOrDefault()},order.Products.SelectMany(o => o.Value).Select(p => new ModelWithColors
                    {
                        ModelVendorCodeSbs = p.GroupKey.ModelVendorCodeSbs,
                        ColorModelVendorCodes = new List<string>{p.GroupKey.ColorModelVendorCodeSbs}
                    }).Distinct().ToList(), order.ProcurementId.GetValueOrDefault()), cancellationToken);
            }
        }

        private async Task ProcessCreated(BusMessages<Order> messages, CancellationToken cancellationToken)
        {
            var createdOrders = messages.Messages.Where(m => m.EventType == CrudEventType.Create).Select(m => m.NewState)
                .ToList();

            var procurementsToClear = createdOrders.Select(o => o.ProcurementId).Select(p => p.GetValueOrDefault()).Distinct()
                .ToList();
            if (!procurementsToClear.Any())
                return;
            var procurements = await _mediator.Send(new GetProcurementsByIdsQuery(procurementsToClear), cancellationToken);

            var selectionsToClear = procurements.SelectMany(p => p.Selections).ToList();

            foreach (var selection in selectionsToClear)
            {
                
                selection.ClearProducts();
                await _mediator.Send(new UpsertSelectionQuery(selection), cancellationToken);
            }
        }
    }
}