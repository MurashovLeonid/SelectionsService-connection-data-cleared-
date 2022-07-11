using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using JetBrains.Annotations;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Superbrands.Bus.Contracts.CSharp;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Libs.DDD.EfCore.Extensions;
using Superbrands.Selection.Bus.DiffComparers;
using Superbrands.Selection.Domain.Events;
using Superbrands.Selection.Domain.Procurements;
using Superbrands.Selection.Infrastructure.Abstractions;

namespace Superbrands.Selection.Bus.Handlers
{
    internal class SendProcurementsToBusNotificationHandler : IDomainEventsNotificationHandler<ProcurementApprovedEvent>,
        IDomainEventsNotificationHandler<ProcurementCreatedEvent>, IDomainEventsNotificationHandler<ProcurementUpdatedEvent>,
        INotificationHandler<ProcurementDeletedEvent>, IDomainEventsNotificationHandler<ProcurementSelectionAddedEvent>
    {
        private readonly IBus _bus;
        private readonly ILogger<SendProcurementsToBusNotificationHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SendProcurementsToBusNotificationHandler(IBus bus,
            ILogger<SendProcurementsToBusNotificationHandler> logger, IMapper mapper,

        [NotNull] IHttpContextAccessor httpContextAccessor)
        {
            _bus = bus;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public Task Handle(ProcurementApprovedEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Procurement with id {@event.Procurement.Id} was approved and send to bus");
            return Handle(@event.Procurement, CrudEventType.Other, cancellationToken);
        }

        public Task Handle(ProcurementCreatedEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Procurement with id {@event.CurrentState.Id} was created");
            return Handle(@event.CurrentState, CrudEventType.Create, cancellationToken);
        }

        public async Task Handle(ProcurementUpdatedEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Procurement with id {@event.CurrentState.Id} was updated");

            await Handle(@event.CurrentState, CrudEventType.Update, cancellationToken,
                @event.OriginalState);
        }

        public Task Handle(ProcurementDeletedEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Procurement with id {@event.CurrentState.Id} was deleted");
            return Handle(@event.CurrentState, CrudEventType.Deleted, cancellationToken);
        }

        /// <inheritdoc />
        public async Task Handle(DomainEventsOfType<ProcurementApprovedEvent> notification, CancellationToken cancellationToken)
        {
            foreach (var @event in notification.Events) await Handle(@event, cancellationToken);
        }

        /// <inheritdoc />
        public async Task Handle(DomainEventsOfType<ProcurementCreatedEvent> notification, CancellationToken cancellationToken)
        {
            foreach (var @event in notification.Events) await Handle(@event, cancellationToken);
        }

        /// <inheritdoc />
        public async Task Handle(DomainEventsOfType<ProcurementUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            foreach (var @event in notification.Events) await Handle(@event, cancellationToken);
        }

        private async Task Handle(Procurement currentState, CrudEventType eventType, CancellationToken cancellationToken,
            Procurement originalState = default)
        {
            if (currentState.Id == default)
                throw new ArgumentNullException(nameof(currentState.Id));

            _httpContextAccessor.TryGetCurrentUserId(out var userId);

            var messages = new BusMessages<Superbrands.Bus.Contracts.CSharp.MsSelections.Procurement.Procurement>();
            switch (eventType)
            {
                case CrudEventType.Create:
                case CrudEventType.Deleted:
                case CrudEventType.Other:
                    var procurementBus = ProcurementComparer.MapWithoutChanges(currentState, eventType, _mapper);
                    var procurementBusMessage =
                        new BusMessage<Superbrands.Bus.Contracts.CSharp.MsSelections.Procurement.Procurement>
                            (procurementBus, eventType, userId);
                    messages.Messages.Add(procurementBusMessage);
                    break;
                case CrudEventType.Update:
                var procurementBusDto = new ProcurementComparer(originalState, currentState, eventType, _mapper)
                        .GetBusContract(userId);
                    messages.Messages.Add(procurementBusDto);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            }

            await messages.SendToQueue(_bus, cancellationToken);
        }

        public async Task Handle(DomainEventsOfType<ProcurementSelectionAddedEvent> notification, CancellationToken cancellationToken)
        {
            foreach (var @event in notification.Events)
            {
                await Handle(@event.CurrentState, CrudEventType.Update, cancellationToken, @event.OriginalState);
            }
        }
    }
}