using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Mapster;
using MapsterMapper;
using Superbrands.Bus.Contracts.CSharp;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Domain.Procurements;
using Xunit;

namespace Superbrands.Selection.UnitTests
{
    public class BusMappingTests : TestBase
    {
        [Fact]
        public void ConvertSelectionToBus()
        {
            var domainPartner = _fixture.Create<Selection.Domain.Selections.Selection>();
            var busPartner = domainPartner.Adapt<Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.Selection>();
            var comparisonResult = _compareLogic.Compare(domainPartner, busPartner);

            if (comparisonResult.Differences.Any())
                throw new Exception(comparisonResult.DifferencesString);
        }

        [Fact]
        public void CreateDifferencesSelection()
        {
            var colorModelMeta = new Superbrands.Selection.Domain.Selections.ColorModelMeta("b22-c", 1, "b22-c-a",
                ColorModelStatus.Archive, ColorModelPriority.PriorityA, "eur");
            var colorModelMeta1 = new Superbrands.Selection.Domain.Selections.SelectionPurchaseSalePointKey(1, 1, 1);

            var selection1 = new Selection.Domain.Selections.Selection(1, 1, SelectionStatus.InProgress);
            selection1.AddColorModel(colorModelMeta);
            selection1.AddPurchaseSalePointKeys(new List<Superbrands.Selection.Domain.Selections.SelectionPurchaseSalePointKey>
                {colorModelMeta1, colorModelMeta1});

            var selection = new Selection.Domain.Selections.Selection(2, 2, SelectionStatus.InProgress) {Id = 1};
            selection1.Id = 2;

            var originalStateObjects = new List<Selection.Domain.Selections.Selection> {selection1};
            var currentStateObjects = new List<Selection.Domain.Selections.Selection> {selection};

            var selectionDifferences =
                ChangedEntitiesCollection<Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.Selection>
                    .Create(originalStateObjects, currentStateObjects, s => s.Id, new Mapper(TypeAdapterConfig.GlobalSettings));
        }

        [Fact]
        public void MapSelectionToBus()
        {
            var colorModelMeta = new Superbrands.Selection.Domain.Selections.ColorModelMeta("b22-c", 1, "b22-c-a",
                ColorModelStatus.Archive, ColorModelPriority.PriorityA, "eur");
            var selectionPurchaseSalePointKey = new Superbrands.Selection.Domain.Selections.SelectionPurchaseSalePointKey(1, 1, 1);
            var selectionPurchaseSalePointKey2 = new Superbrands.Selection.Domain.Selections.SelectionPurchaseSalePointKey(1, 1, 2);

            var selection1 = new Selection.Domain.Selections.Selection(1, 1, SelectionStatus.InProgress);
            selection1.AddPurchaseSalePointKeys(new List<Superbrands.Selection.Domain.Selections.SelectionPurchaseSalePointKey>
                {selectionPurchaseSalePointKey, selectionPurchaseSalePointKey2});
            selection1.AddColorModel(colorModelMeta);
           

            var bus = selection1.Adapt<Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.Selection>();

            bus.Should().NotBeNull();
        }

        [Fact]
        public void ConvertProcurementToBus()
        {
            _compareLogic.Config
                .IgnoreProperty<Superbrands.Bus.Contracts.CSharp.MsSelections.Procurement.Procurement>(s => s.Selections);
            _compareLogic.Config
                .IgnoreProperty<Superbrands.Bus.Contracts.CSharp.MsSelections.Procurement.Procurement>(s => s.SalePoints);
            _compareLogic.Config
                .IgnoreProperty<Superbrands.Bus.Contracts.CSharp.MsSelections.Procurement.Procurement>(s => s.ProcurementKeySets);

            var domainProcurement = _fixture.Create<Procurement>();
            var busProcurement = domainProcurement.Adapt<Superbrands.Bus.Contracts.CSharp.MsSelections.Procurement.Procurement>();

            _compareLogic.Config.IgnoreProperty<Procurement>(s => s.SeasonId);
            var comparisonResult = _compareLogic.Compare(domainProcurement, busProcurement);

            if (comparisonResult.Differences.Any())
                throw new Exception(comparisonResult.DifferencesString);
        }
    }
}