using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Superbrands.Libs.RestClients.Members;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Application.Selection
{
    internal class GetUserSelectionsQueryHandler : IRequestHandler<GetUserSelectionsQuery, IEnumerable<Domain.Selections.Selection>>
    {
        private readonly ISelectionRepository _selectionRepository;
        private readonly IMembersMembersClient _membersClient;

        public GetUserSelectionsQueryHandler(ISelectionRepository selectionRepository,
            IMembersMembersClient membersClient)
        {
            _selectionRepository = selectionRepository;
            _membersClient = membersClient;
        }

        public async Task<IEnumerable<Domain.Selections.Selection>> Handle(GetUserSelectionsQuery request,
            CancellationToken cancellationToken)
        {
            var partnerIds = await GetUserData(request.UserId, cancellationToken);

            var selections = new List<SelectionDalDto>();
            foreach (var partnerId in partnerIds)
            {
                var selectionsInPartner = await _selectionRepository
                    .GetAvailableSelections(request.UserId, partnerId, 0, null, cancellationToken);
                selections.AddRange(selectionsInPartner);
            }

            return selections.Select(x => x.ToDomain());
        }

        private async Task<IEnumerable<long>> GetUserData(long userId, CancellationToken cancellationToken)
        {
            var member = await _membersClient.Members_GetByIdAsync(userId, cancellationToken);
            var partnerId = member.PartnerId;
            return new[]{ partnerId };
        }
    }
}