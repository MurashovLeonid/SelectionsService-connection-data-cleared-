using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Superbrands.Bus.Contracts.CSharp.MsSelections.Enums;
using Superbrands.Bus.Contracts.CSharp.MsSelections.Selections;
using Superbrands.Libs.HttpClient;
using Superbrands.Libs.RestClients.Members;
using Superbrands.Libs.RestClients.Partners;
using Superbrands.Libs.RestClients.PartnersInfrastructure;
using Superbrands.Libs.RestClients.Pim;
using Superbrands.Libs.RestClients.Seasons;
using Superbrands.Libs.RestClients.SSO;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Domain.Events;
using Superbrands.Selection.Domain.Procurements;
using Superbrands.Selection.Domain.SalePoints;
using Superbrands.Selection.Domain.Selections;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;
using OperationLog = Superbrands.Libs.DDD.Abstractions.OperationLog;
using ProcurementStage = Superbrands.Selection.Domain.Enums.ProcurementStage;

namespace Superbrands.Selection.Application.Procurement
{
    class GenerateProcurementsHandler : IRequestHandler<GenerateProcurementsForPartnersQuery,
        List<Domain.Procurements.Procurement>>, IRequestHandler<GenerateProcurementsForSeasonQuery,
        List<Domain.Procurements.Procurement>>, IRequestHandler<GenerateProcurementsForSalePointsQuery,
        List<Domain.Procurements.Procurement>>, IRequestHandler<DeleteSalePointsFromProcurementQuery,
        List<Domain.Procurements.Procurement>>

    {
        private readonly ISelectionRepository _selectionRepository;
        private readonly IProcurementRepository _repository;
        private readonly IPartnersInfrastructureSalepointClient _salePointClient;
        private readonly IPartnersPartnersClient _partnersClient;
        private readonly ISeasonsSeasoncapsuleClient _seasonsClient;
        private readonly ISSOUserentitiesbindClient _entityBindingsClient;
        private readonly IMembersMembersClient _membersClient;
        private readonly ISSOUserClient _userClient;
        private readonly IMediator _mediator;

        private ICollection<PartnerDto> _partners;
        private ICollection<SeasonCapsuleDto> _seasons;
        private Dictionary<string, ProcurementDalDto> _procurements;
        private Dictionary<long, List<SalePointDto>> _partnerSalePoints;

        private const int BuyerRoleId = 13;
        private const string PurchaseKeyEntityName = "Superbrands.Bus.Contracts.CSharp.MSPurchaseKeys.PurchaseKey";
        private const string SalePointEntityName = "Superbrands.Bus.Contracts.CSharp.MsPartnersInfrastructure.SalePoint.SalePoint";

        public GenerateProcurementsHandler(IProcurementRepository repository, IPartnersInfrastructureSalepointClient salePointClient,
            IPartnersPartnersClient partnersClient, ISeasonsSeasoncapsuleClient seasonsClient, ISSOUserentitiesbindClient entityBindingsClient,
            IMembersMembersClient membersClient, [NotNull] ISSOUserClient userClient, Superbrands.Libs.RestClients.Partners.IPartnersRelationsClient relationsClient
            ,ISelectionRepository selectionRepository, [NotNull] IMediator mediator)

        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _salePointClient = salePointClient ?? throw new ArgumentNullException(nameof(salePointClient));
            _partnersClient = partnersClient ?? throw new ArgumentNullException(nameof(partnersClient));
            _seasonsClient = seasonsClient ?? throw new ArgumentNullException(nameof(seasonsClient));
            _entityBindingsClient = entityBindingsClient ?? throw new ArgumentNullException(nameof(entityBindingsClient));
            _membersClient = membersClient ?? throw new ArgumentNullException(nameof(membersClient));
            _userClient = userClient ?? throw new ArgumentNullException(nameof(userClient));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _selectionRepository = selectionRepository ?? throw new ArgumentNullException(nameof(selectionRepository));
        }

        public async Task<List<Domain.Procurements.Procurement>> Handle(GenerateProcurementsForPartnersQuery request,
            CancellationToken cancellationToken)
        {
            await LoadData(request.PartnersIds, request.SeasonCapsulesIds, cancellationToken:cancellationToken);
            return await GenerateProcurements(cancellationToken);
        }

        public async Task<List<Domain.Procurements.Procurement>> Handle(DeleteSalePointsFromProcurementQuery request,CancellationToken cancellationToken)
        {
            var procurementList = await _repository.GetBySalePointIds(request.SalePointIds, cancellationToken);
            return await RemoveSalePointFromProcurement(procurementList, request.SalePointIds, cancellationToken);                     
        }

        public async Task<List<Domain.Procurements.Procurement>> Handle(GenerateProcurementsForSeasonQuery request, CancellationToken cancellationToken)
        {
            var season = await _seasonsClient.SeasonCapsule_GetByIdAsync(request.SeasonCapsulesId, cancellationToken);
            var salepoints = await _salePointClient.SalePoint_GetPagedAsync(1, int.MaxValue,
                SalePointStatus.Open, season.SeasonCapsuleKindId, cancellationToken);
            var partnerIds = salepoints.Results.Select(s => s.PartnerId).Distinct().ToList();
            await LoadData(partnerIds, new List<long>{request.SeasonCapsulesId}, salepoints.Results,null, cancellationToken);
            return await GenerateProcurements(cancellationToken);
        }
        public async Task<List<Domain.Procurements.Procurement>> Handle(GenerateProcurementsForSalePointsQuery request,CancellationToken cancellationToken) 
        {
            _salePointClient.AuthAsMicroservice();
            _seasonsClient.AuthAsMicroservice();

            var salePoints = await _salePointClient.SalePoint_GetByIdsAsync(request.SalePointIds, cancellationToken);
            var salePointSeasonCapsuleKindIds = salePoints.Select(x => x.SeasonCapsuleKindIds.ToList()).ToList();
            var requiredSeasonCapsuleKindIds = GetListOfSeasonKindIds(salePointSeasonCapsuleKindIds);
            var requiredSeasons = await GetRequiredSeasonIds(requiredSeasonCapsuleKindIds, cancellationToken);

            await LoadData(salePoints.Select(x=> x.PartnerId).ToList(), requiredSeasons.Select(x=> x.Id).ToList(), salePoints, requiredSeasons, cancellationToken);


            return await GenerateProcurements(cancellationToken);
        }

        private async Task<List<Domain.Procurements.Procurement>> GenerateProcurements(CancellationToken cancellationToken)
        {
            var procurementsDalDtos = new List<ProcurementDalDto>();
            var updatedProcurements = new List<ProcurementDalDto>();
            foreach (var partner in _partners)
            foreach (var sc in _seasons)
            {
                var segments = _partnerSalePoints[partner.Id].Where(x => x.MainSegmentId.HasValue).Select(s => s.MainSegmentId.Value).Distinct().ToList();
                foreach (var segment in segments)
                {
                    //Если на какое-то сочетание партнер + сезонная капсула нет ТП c видом сезонной капсулы, то такую закупку генерировать не надо.
                    if (!IsSalePointExist(partner.Id, sc.SeasonCapsuleKindId))
                    {
                        continue;
                    }
                    var salePoints = _partnerSalePoints[partner.Id]
                            ?.Where(x => x.SeasonCapsuleKindIds?.Any() == true && x.SeasonCapsuleKindIds.Contains(sc.SeasonCapsuleKindId) && x.MainSegmentId == segment)
                            ?.ToList() ?? null;

                    //Если существуют закупки, у которых совпадает партнер, сезон и сегмент, то все ТП добавляем в эту закупку
                    if (ProcurementAlreadyExist(segment, sc.Id, partner.Id) == true && salePoints != null && salePoints.Any())
                    {
                        var requiredProcurementsList = _procurements.Where(x => x.Key == GetProcurementKey(segment, sc.Id, partner.Id)).Select(x => x.Value).ToList();
                        foreach (var requiredProcurement in requiredProcurementsList)
                        {
                            var procurementToUpdate = await AddSalePointsToProcurement(requiredProcurement, partner, salePoints, cancellationToken);
                            _repository.Update(procurementToUpdate);
                            updatedProcurements.Add(requiredProcurement);
                        }

                        continue;
                    }

                    if (!salePoints.Any())
                    {
                        continue;
                    }

                    var procurement = await CreateProcurement(partner, segment, sc, salePoints, cancellationToken);
                    var dalDto = ProcurementDalDto.FromDomain(procurement);
                    procurementsDalDtos.Add(dalDto);
                        _procurements.Add(GetProcurementKey(segment, sc.Id, partner.Id), dalDto);

                }
            }

            await _repository.AddRange(procurementsDalDtos, cancellationToken);
            await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);

            var resultProcurementsList = new List<ProcurementDalDto>();

            resultProcurementsList.AddRange(procurementsDalDtos);
            resultProcurementsList.AddRange(updatedProcurements);

            return resultProcurementsList.Select(x => x.ToDomain<ProcurementUpdatedEvent, Domain.Procurements.Procurement>())
                .ToList();
        }
        

        private async Task<ProcurementDalDto> AddSalePointsToProcurement(ProcurementDalDto procurement, PartnerDto partner, List<SalePointDto> salePoints, CancellationToken cancellationToken)
        {

            var procurementDomain = procurement.ToDomain<Domain.Procurements.Procurement>();
            procurementDomain.AddSalePoints(salePoints.Select(x => new Superbrands.Selection.Domain.SalePoints.SalePoint(x.Id, true)).ToList());
            if (await CheckBuyersOnSalePoint(partner, salePoints.Select(x => x.Id).First(), cancellationToken))
            {
                var selections = await UpsertSelectionsForProcurement(partner, procurementDomain, cancellationToken);
                selections.ForEach(selection => procurementDomain.AddSelection(selection));
            }          
            return ProcurementDalDto.FromDomain(procurementDomain);
        }
        private async Task<List<Domain.Selections.Selection>> UpsertSelectionsForProcurement(PartnerDto partner, Domain.Procurements.Procurement procurement, CancellationToken cancellationToken)
        {

            var newSelections = new List<Domain.Selections.Selection>();
            // получаем все отборки для закупки
            var procurementSelections = await GetSelectionsForProcurement(procurement.Id, cancellationToken);

            _membersClient.AuthAsMicroservice();
            var partnerMembers =
                await _membersClient.Members_GetMemberPagedByPartnerIdAsync(partner.Id, 1, 100, cancellationToken);
            var membersPurchaseKeys =
                await GetMembersPurchaseKeys(partnerMembers.Results.Select(x => x.Id).ToList(), cancellationToken);

            foreach (var memberPurchaseKey in membersPurchaseKeys)
            {

                //выбираем все связки SalePoint<-->PurchaseKey для определенного закупщика
                var selectionsPurchasesKeys = memberPurchaseKey.Value
                       .Where(x => x.PurchaseKeyId > 0 && x.SalePointId > 0).Select(e =>
                       new Domain.Selections.SelectionPurchaseSalePointKey(default, e.SalePointId, e.PurchaseKeyId))?.ToList() ?? null;
                if (selectionsPurchasesKeys != null && !selectionsPurchasesKeys.Any())
                {
                    continue;
                }
               
                var selection = new Domain.Selections.Selection();

                // определяем отборку, в которой этот закупщик участвует
                var selectionToUpdate = procurementSelections.Where(x => x.BuyerId == memberPurchaseKey.Key)?.FirstOrDefault() ?? null;
                if (selectionToUpdate != null)
                {
                    //добавляем к отборке новый SelectionSalePointPurchaseKey
                    UpdateSelections(selectionToUpdate, memberPurchaseKey);
                    continue;
                }
                
                selection = new Domain.Selections.Selection(memberPurchaseKey.Key, default,
                    Domain.Enums.SelectionStatus.SelectionIsEmpty);

                selection.AddPurchaseSalePointKeys(selectionsPurchasesKeys);

                newSelections.Add(selection);
            }

            return newSelections;
        }

        private async Task<List<Domain.Selections.Selection>> GetSelectionsForProcurement(long procurementId, CancellationToken cancellationToken)
        {
            var result = await _selectionRepository.GetByProcurementId(procurementId, cancellationToken);
            return result.Select(x=> x.ToDomain()).ToList();
        }
        private void UpdateSelections(Domain.Selections.Selection selectionToUpdate, KeyValuePair<long, HashSet<BindedEntities>> keys)
        {          
            var bindsToCheck = keys.Value.ToList();
            var bindsToCreate = bindsToCheck
                .Where(x => x.PurchaseKeyId > 0 && x.SalePointId > 0 &&(selectionToUpdate.SelectionPurchaseSalePointKeys.Where(y => y.PurchaseKeyId == x.PurchaseKeyId && y.SalePointId == x.SalePointId)?.ToList() ?? null) == null)?.
                ToList() ?? null;
            if (bindsToCreate != null)
            {
                var keysToAdd = bindsToCreate.Select(x => new Domain.Selections.SelectionPurchaseSalePointKey(selectionToUpdate.Id, x.SalePointId, x.PurchaseKeyId));
                selectionToUpdate.AddPurchaseSalePointKeys(keysToAdd);
            }
            
        }
        private async Task<Domain.Procurements.Procurement> CreateProcurement(PartnerDto partner,
            long segmentId, SeasonCapsuleDto sc, List<SalePointDto> salePoints,
            CancellationToken cancellationToken)
        {
            
            var counterpartyConditions = await _mediator.Send(new CreateCounterpartyConditionQuery(partner.Id, salePoints.Select(s => s.CounterpartyId).Where(c => c.HasValue).Select(c => c.Value).Distinct()), cancellationToken);
            var procurementSalePoints = salePoints.Select(x => new Superbrands.Selection.Domain.SalePoints.SalePoint(x.Id, true)).ToList();
            var brandsIds = salePoints.SelectMany(x => x.BrandIds ?? Enumerable.Empty<long>()).Distinct();
            var procurementStage = _seasonCapsuleStatusToProcurementStageDict[sc.Status];
            var (partnerTeam, sbsTeam) = await GetProcurementTeams(partner, salePoints, cancellationToken);
            var procurement = new Domain.Procurements.Procurement(Guid.NewGuid().ToString(), sc.Id, partner.Id,
                Domain.Enums.ProcurementKind.Wholesale,
                procurementStage, Domain.Enums.ProcurementStatus.ReadyForSelection, procurementSalePoints,
                new Dictionary<long, decimal>(), counterpartyConditions,
                segmentId);
            procurement.SetTeams(partnerTeam, sbsTeam);

            
            if(await CheckBuyersOnSalePoint(partner,salePoints.Select(x=> x.Id).First(),cancellationToken))
            {
                var selections = await CreateSelectionsForPartnerMembers(partner, cancellationToken);
                selections.ForEach(selection => procurement.AddSelection(selection));
            }    
            
            return procurement;
        }

        private async Task<bool> CheckBuyersOnSalePoint(PartnerDto partner, long salePointId, CancellationToken cancellationToken)
        {
            _membersClient.AuthAsMicroservice();
            var partnerMembers =
                await _membersClient.Members_GetMemberPagedByPartnerIdAsync(partner.Id, 1, 100, cancellationToken);
            var membersPurchaseKeys =
                await GetMembersPurchaseKeys(partnerMembers.Results.Select(x => x.Id).ToList(), cancellationToken);
            var bindedEntitiesToCheckList = membersPurchaseKeys.Where(x => (x.Value.Where(y => y.SalePointId == salePointId)?.ToList()?.FirstOrDefault()) != null)?.Select(x => x.Value)?.FirstOrDefault() ?? null;
            return bindedEntitiesToCheckList != null;
        }
        private async Task<(List<long> partnerTeam, List<long> sbsTeam)> GetProcurementTeams(PartnerDto partner, List<SalePointDto> salePoints, CancellationToken cancellationToken)
        {
            _entityBindingsClient.AuthAsMicroservice();
            var salePointUserIds = new List<int>();
            foreach (var salePointDto in salePoints)
            {
                salePointUserIds.AddRange(await _entityBindingsClient.GetBoundariesForEntityAsync(salePointDto.Id.ToString(),
                    SalePointEntityName, cancellationToken));
            }

            _userClient.AuthAsMicroservice();
            var salepointUsers =
                await _userClient.GetUsersByIdsAsync(salePointUserIds.Select(q => (long) q), cancellationToken);
            _membersClient.AuthAsMicroservice();
            var salepointMembers = await _membersClient.Members_GetByIdsAsync(salepointUsers.Where(x => x.MemberId.HasValue)
                .Select(u => u.MemberId.Value));

            var partnerTeam = salepointMembers.Where(m => m.PartnerId == partner.Id)
                .Select(m => m.Id)
                .ToList();
            var sbsTeam = salepointMembers.Select(m => m.Id).Except(partnerTeam).ToList();
            return (partnerTeam, sbsTeam);
        }
       

        private async Task<List<Domain.Selections.Selection>> CreateSelectionsForPartnerMembers(PartnerDto partner,
            CancellationToken cancellationToken)
        {
            var selections = new List<Domain.Selections.Selection>();
            var partnerMembers =
                await _membersClient.Members_GetMemberPagedByPartnerIdAsync(partner.Id, 1, 100, cancellationToken);
            var membersPurchaseKeys =
                await GetMembersPurchaseKeys(partnerMembers.Results.Select(x => x.Id).ToList(), cancellationToken);

            foreach (var memberPurchaseKey in membersPurchaseKeys)
            {
                var selection = new Domain.Selections.Selection(memberPurchaseKey.Key, default,
                    Domain.Enums.SelectionStatus.SelectionIsEmpty);

                var selectionsPurchasesKeys = memberPurchaseKey.Value
                    .Where(x => x.PurchaseKeyId > 0 && x.SalePointId > 0).Select(e =>
                    new Domain.Selections.SelectionPurchaseSalePointKey(default, e.SalePointId, e.PurchaseKeyId));

                if (!selectionsPurchasesKeys.Any())
                {
                    //если нет привязок, не создаем отборку
                    continue;
                }

                selection.AddPurchaseSalePointKeys(selectionsPurchasesKeys);

                selections.Add(selection);
            }

            return selections;
        }

        private async Task LoadData(List<long> partnersIds, List<long> seasonCapsulesIds = null, IEnumerable<SalePointDto> salePoints = null, IEnumerable<SeasonCapsuleDto> seasons = null, CancellationToken cancellationToken = default)
        {
            var getSeasonsCapsulesTask = (seasonCapsulesIds != null && seasonCapsulesIds.Any())
                ? _seasonsClient.SeasonCapsule_GetByIdsAsync(seasonCapsulesIds, cancellationToken)
                : _seasonsClient.SeasonCapsule_GetAllAsync(cancellationToken);

            _partnersClient.AuthAsMicroservice();

            var getPartnersTask = _partnersClient.Partners_GetByIdsAsync(partnersIds, cancellationToken);

            var getProcurementsTask = _repository.GetAll(cancellationToken);

            await Task.WhenAll(getSeasonsCapsulesTask, getPartnersTask, getProcurementsTask);


            _partners = await getPartnersTask;
            _seasons = (ICollection<SeasonCapsuleDto>)seasons ?? await getSeasonsCapsulesTask;

            _procurements = (await getProcurementsTask).GroupBy(x => GetProcurementKey(x.SegmentId, x.SeasonId,  x.PartnerId))
                .ToDictionary(x => x.Key, x => x.FirstOrDefault());

            _salePointClient.AuthAsMicroservice();
            _partnerSalePoints = 
                (salePoints ?? await _salePointClient.SalePoint_GetByPartnersIdsAsync(_partners.Select(x => x.Id).ToList(), cancellationToken))
                .GroupBy(x => x.PartnerId).ToDictionary(x => x.Key, x => x.ToList());
                        
        }

        private bool IsSalePointExist(long partnerId, long seasonCapsuleKindId)
        {
            return _partnerSalePoints.TryGetValue(partnerId, out var sps) &&
                   sps.Any(x => x.SeasonCapsuleKindIds?.Contains(seasonCapsuleKindId) == true);
        }
       
        private bool ProcurementAlreadyExist(long? segmentId, long seasonCapsuleId, long partnerId)
        {
            var key = GetProcurementKey(segmentId, seasonCapsuleId, partnerId);
            var result = _procurements.ContainsKey(key);
            return result;
        }

        private string GetProcurementKey(long? segmentId, long seasonCapsuleId, long partnerId)
            => $"{segmentId}-{seasonCapsuleId}-{partnerId}";

        private readonly Dictionary<SeasonCapsuleStatus, ProcurementStage> _seasonCapsuleStatusToProcurementStageDict =
            new Dictionary<SeasonCapsuleStatus, ProcurementStage>()
            {
                [SeasonCapsuleStatus.InPreparation] = ProcurementStage.None,
                [SeasonCapsuleStatus.Active] = ProcurementStage.PreOrder,
                //[SeasonCapsuleStatus.PreSeason] = ProcurementStage.PreSeason,
                [SeasonCapsuleStatus.Stop] = ProcurementStage.FreeWarehouse
            };
        
        private async Task<Dictionary<long, HashSet<BindedEntities>>>  GetMembersPurchaseKeys(List<long> membersIds, CancellationToken cancellationToken)
        {
            try
            {
                _userClient.AuthAsMicroservice();
                var users = await _userClient.GetUsersByMembersIdsAsync(membersIds, cancellationToken);
                var usersDict = users.Where(u => u.MemberId.HasValue).ToDictionary(x => x.Id, x => x.MemberId.Value);

                _entityBindingsClient.AuthAsMicroservice();
                var roleEntityBindings = await _entityBindingsClient.GetUsersEntitiesBindsForRolesAsync(users.Select(x => (long)x.Id).ToList(), new List<long> {BuyerRoleId},
                        cancellationToken);
                var membersBindings = roleEntityBindings
                    .Where(x => usersDict.ContainsKey((int) x.UserId));
                var memberPurchaseKeys =
                    membersBindings.ToDictionary(x => usersDict[(int) x.UserId],
                        x => x.EntityBinds.Select(b  => b.Entities
                            .Where(e => e.EntityAssemblyQualifiedName is PurchaseKeyEntityName or SalePointEntityName)
                            .Aggregate(new BindedEntities(), (entities, entity) =>
                            {
                                switch (entity.EntityAssemblyQualifiedName)
                                {
                                    case PurchaseKeyEntityName:
                                        entities.PurchaseKeyId = long.Parse(entity.EntityId);
                                        return entities;
                                    case SalePointEntityName:
                                        entities.SalePointId = long.Parse(entity.EntityId);
                                        return entities;
                                    default: return entities;
                                }
                            }) ).ToHashSet());

                return memberPurchaseKeys;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        private async Task<List<SeasonCapsuleDto>> GetRequiredSeasonIds(List<long> seasonCapsuleKindIds, CancellationToken cancellationToken)
        {
            var seasonCapsulesList = await _seasonsClient.SeasonCapsule_GetAllAsync(cancellationToken);
            return seasonCapsulesList
                .Where(x => x.Status == SeasonCapsuleStatus.Active && seasonCapsuleKindIds.Contains(x.SeasonCapsuleKindId))?
                .ToList() ?? null;
        }
        private  List<long> GetListOfSeasonKindIds(List<List<long>> list)
        {
            var result = new List<long>();
            for (int i = 0; i <= list.Count - 1; i++)
            {
                result.AddRange(list[i]);
            }
           
            return result.Distinct().ToList();
        }
        private async Task<List<Domain.Procurements.Procurement>> RemoveSalePointFromProcurement(List<ProcurementDalDto> procurementsList, List<long> salePointIds, CancellationToken cancellationToken)
        {
            for (int i = 0; i <= procurementsList.Count - 1; i++)
            {
                var deletedSalePoints = procurementsList[i].SalePoints?
                                                            .Where(x => salePointIds.Contains(x.Id))?
                                                            .ToList() ?? null;

                if (deletedSalePoints != null && deletedSalePoints.Any())
                {
                    foreach (var deletedSalePoint in deletedSalePoints)
                    {
                        procurementsList[i].SalePoints.Remove(deletedSalePoint);
                        await _repository.Update(procurementsList[i], cancellationToken);
                    }
                }
            }
            await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return procurementsList.Select(x => x.ToDomain<ProcurementUpdatedEvent, Domain.Procurements.Procurement>())
                .ToList();
        }
        

    }
}