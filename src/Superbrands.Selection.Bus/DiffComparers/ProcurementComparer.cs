using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mapster;
using MapsterMapper;
using Superbrands.Bus.Contracts.CSharp;
using Superbrands.Bus.Contracts.CSharp.MsSelections.Procurement;
using Superbrands.Bus.Contracts.CSharp.MsSelections.Selections;
using Procurement = Superbrands.Selection.Domain.Procurements.Procurement;

namespace Superbrands.Selection.Bus.DiffComparers
{
    public class ProcurementComparer
    {
        private readonly Procurement _originalState;
        private readonly Procurement _currentState;
        private readonly CrudEventType _eventType;
        private readonly IMapper _mapper;

        public ProcurementComparer([NotNull] Procurement originalState, [NotNull] Procurement currentState,
            CrudEventType eventType, [NotNull] IMapper mapper)
        {
            _originalState = originalState;
            _currentState = currentState ?? throw new ArgumentNullException(nameof(currentState));
            _eventType = eventType;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public BusMessage<Superbrands.Bus.Contracts.CSharp.MsSelections.Procurement.Procurement> GetBusContract(long operatorId)
        {
            var currentProcurementBus = _mapper.Map<Superbrands.Bus.Contracts.CSharp.MsSelections.Procurement.Procurement>
                (_currentState);
            currentProcurementBus.Selections = GetSelectionsChanges(_currentState, _originalState);
            
            var originalProcurementBus = _mapper.Map<Superbrands.Bus.Contracts.CSharp.MsSelections.Procurement.Procurement>
                (_originalState);
            var originalSelections = new ChangedEntitiesCollection<Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.Selection>();
            originalSelections.AddUpdated(_originalState.Selections.Select(s => _mapper.Map<Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.Selection>(s)));
            originalProcurementBus.Selections = originalSelections;
            
            return new BusMessage<Superbrands.Bus.Contracts.CSharp.MsSelections.Procurement.Procurement>(currentProcurementBus,
                _eventType, operatorId, originalProcurementBus);
        }

        private ChangedEntitiesCollection<Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.Selection> GetSelectionsChanges
            (Procurement currentState, Procurement originalState)
        {
            var selectionDifferences =
                ChangedEntitiesCollection<Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.Selection>
                    .Create(originalState.Selections, currentState.Selections, s => s.Id, _mapper);

            SetColorModelsChanges(currentState, originalState, selectionDifferences);

                return selectionDifferences;
        }

        private void SetColorModelsChanges(Procurement currentState, Procurement originalState,
            ChangedEntitiesCollection<Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.Selection>
                selectionDifferences)
        {
            foreach (var (vEventType, differences) in selectionDifferences)
            {
                foreach (var diff in differences)
                {
                    var originalStateSelection = originalState.Selections.FirstOrDefault(s => s.Id == diff.Id);//может не быть если отборка новая
                    var newStateSelection = currentState.Selections.First(s => s.Id == diff.Id);

                    var colorModelMetasChanges = ChangedEntitiesCollection<ColorModelMeta>.Create(
                        originalStateSelection?.ColorModelMetas ?? Enumerable.Empty<Superbrands.Selection.Domain.Selections.ColorModelMeta>(), newStateSelection.ColorModelMetas, s => s.Id, _mapper);

                    var selectionPurchaseSalePointKeys = ChangedEntitiesCollection<SelectionPurchaseSalePointKey>.Create(
                        originalStateSelection?.SelectionPurchaseSalePointKeys ?? Enumerable.Empty<Superbrands.Selection.Domain.Selections.SelectionPurchaseSalePointKey>(), newStateSelection.SelectionPurchaseSalePointKeys, s => s.Id, _mapper);

                    diff.SelectionPurchaseSalePointKeys = selectionPurchaseSalePointKeys;
                    diff.ColorModelMetas = colorModelMetasChanges;
                    SetSizeChanges(currentState, originalState, colorModelMetasChanges);
                }
            }
        }

        private void SetSizeChanges(Procurement currentState, Procurement originalState,
            ChangedEntitiesCollection<ColorModelMeta> colorModelMetasChanges)
        {
            if (colorModelMetasChanges.TryGetValue(CrudEventType.Create, out var createdMetas))
            {
                foreach (var createdMeta in createdMetas)
                {
                    var sizes = GetSelectionSizes(currentState, createdMeta);

                    if (createdMeta.Sizes == null) createdMeta.Sizes = new();
                    createdMeta.Sizes.AddCreated(_mapper.Map<IEnumerable<Size>>(sizes));
                }
            }

            if (colorModelMetasChanges.TryGetValue(CrudEventType.Update, out var updatedMetas))
            {
                foreach (var updatedMeta in updatedMetas)
                {
                    var currentSizes = GetSelectionSizes(currentState, updatedMeta);
                    var originalSizes = GetSelectionSizes(originalState, updatedMeta);

                    updatedMeta.Sizes = ChangedEntitiesCollection<Size>.Create
                        (originalSizes, currentSizes, s => s.Sku, _mapper);
                }
            }

            if (colorModelMetasChanges.TryGetValue(CrudEventType.Deleted, out var deletedMetas))
            {
                foreach (var deletedMeta in deletedMetas.Where(s => s != null))
                {
                    var sizes = GetSelectionSizes(originalState, deletedMeta);
                    if (sizes == null) deletedMeta.Sizes = new();

                    if (sizes != null)
                        deletedMeta?.Sizes?.AddDeleted(_mapper.Map<IEnumerable<Size>>(sizes));
                }
            }

            IEnumerable<Domain.Size> GetSelectionSizes(Procurement procurement, ColorModelMeta colorMeta)
            {
                var selectionId = colorMeta.SelectionId;
                var sizes = procurement.Selections?.FirstOrDefault(s => s.Id == selectionId)
                    ?.ColorModelMetas.FirstOrDefault(s => s.ColorModelVendorCodeSbs == colorMeta.ColorModelVendorCodeSbs)?.Sizes;
                return sizes;
            }
        }

        public static Superbrands.Bus.Contracts.CSharp.MsSelections.Procurement.Procurement MapWithoutChanges(
            Procurement currentState, CrudEventType eventType, IMapper mapper)
        {
            var procurementBus = mapper.Map<Superbrands.Bus.Contracts.CSharp.MsSelections.Procurement.Procurement>(currentState);

            procurementBus.SalePoints = new ChangedEntitiesCollection<SalePoint>
            {
                {
                    eventType, mapper.Map<IEnumerable<SalePoint>>
                        (currentState.SalePoints)
                }
            };

            procurementBus.Selections =
                new ChangedEntitiesCollection<Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.Selection>
                {
                    {
                        eventType, GetSelections(currentState, eventType, mapper)
                    }
                };

            procurementBus.ProcurementKeySets = new ChangedEntitiesCollection<ProcurementKeySet>
            {
                {
                    eventType, mapper.Map<IEnumerable<ProcurementKeySet>>
                        (currentState.ProcurementKeySets)
                }
            };

            return procurementBus;
        }

        private static IEnumerable<Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.Selection> GetSelections
            (Procurement currentState, CrudEventType eventType, IMapper mapper)
        {
            foreach (var selection in currentState.Selections)
            {
                var selectionBus = mapper.Map<Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.Selection>
                    (selection);

                selectionBus.ColorModelMetas = new ChangedEntitiesCollection<ColorModelMeta>
                {
                    {
                        eventType, GetColorModels(selection, eventType, mapper)
                    }
                };

                yield return selectionBus;
            }
        }

        private static IEnumerable<ColorModelMeta> GetColorModels(Domain.Selections.Selection selection, CrudEventType eventType,
            IMapper mapper)
        {
            var selectionColorModelMetas = selection.ColorModelMetas;
            foreach (var modelMeta in selectionColorModelMetas)
            {
                var modelMetaBus = mapper.Map<ColorModelMeta>(modelMeta);

                modelMetaBus.Sizes = new ChangedEntitiesCollection<Size>
                {
                    {
                        eventType, mapper.Map<IEnumerable<Size>>(modelMeta.Sizes)
                    }
                };

                yield return modelMetaBus;
            }
        }
    }
}