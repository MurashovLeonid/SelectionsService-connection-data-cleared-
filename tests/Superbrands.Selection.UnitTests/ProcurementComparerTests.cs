using System.Linq;
using FluentAssertions;
using MapsterMapper;
using Superbrands.Bus.Contracts.CSharp;
using Superbrands.Selection.Bus.DiffComparers;
using Superbrands.Selection.Domain.Procurements;
using Superbrands.Selection.UnitTests.Mocks;
using Xunit;

namespace Superbrands.Selection.UnitTests
{
    public class ProcurementComparerTests : TestBase
    {
        [Fact]
        public void ConvertProcurementToBus_NoMappings_HasNoChanges()
        {
            var domainProcurement = ProcurementDomainStub.GetOne();

            var selectionsDiffs = ChangedEntitiesCollection<Selection.Domain.Selections.Selection>
                .Create(domainProcurement.Selections, ProcurementDomainStub.GetOne().Selections, p => p.Id, null);

            selectionsDiffs.Should().HaveCount(0);
        }

        [Fact]
        public void ConvertProcurementToBus_Mappings_HasNoChanges()
        {
            var domainProcurement = ProcurementDomainStub.GetOne();

            var selectionsDiffs = ChangedEntitiesCollection<Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.Selection>
                .Create(domainProcurement.Selections, ProcurementDomainStub.GetOne().Selections, p => p.Id, new Mapper());

            selectionsDiffs.Should().HaveCount(0);
        }

        [Fact]
        public void CompareTwoProcurements_ComparerUsed_HasNoChanges()
        {
            var domainProcurement = ProcurementDomainStub.GetOne();


        var sut = new ProcurementComparer(domainProcurement, ProcurementDomainStub.GetOne(), CrudEventType.Update,
                new Mapper());

            var changes = sut.GetBusContract(1);

            changes.NewState.Selections.Should().HaveCount(0);
            changes.NewState.ProcurementKeySets.Should().BeEmpty();
            changes.NewState.SalePoints.Should().BeEmpty();
        }

        [Fact]
        public void CompareTwoProcurements_ComparerUsed_HasChanges()
        {
            var domainProcurement = ProcurementDomainStub.GetOne();

            var sut = new ProcurementComparer(domainProcurement, ProcurementDomainStub.GetModified(), CrudEventType.Update,
                new Mapper());

            var changes = sut.GetBusContract(1);

            changes.NewState.Selections.Should().HaveCount(1, "Only update for selections done");
            changes.NewState.Selections.TryGetValue(CrudEventType.Update, out var selectionsUpdates);
            selectionsUpdates.Should().HaveCount(2, "Both selection modified");

            var firstSelectionProductChanges = selectionsUpdates.First().ColorModelMetas;
            var secondSelectionProductChanges = selectionsUpdates.Skip(1).First().ColorModelMetas;

            firstSelectionProductChanges.Should().HaveCount(1, "только одно изменение");
            secondSelectionProductChanges.Should().HaveCount(1, "1 продукт удален");

            secondSelectionProductChanges.Keys.Should().ContainSingle(s => s == CrudEventType.Deleted);

            firstSelectionProductChanges.TryGetValue(CrudEventType.Update, out var firstSelectionChangedProducts).Should()
                .BeTrue();
            firstSelectionChangedProducts.Should().HaveCount(1);

            var sizesOfFirstSelection = firstSelectionChangedProducts.First().Sizes;
            sizesOfFirstSelection.Keys.Should().ContainSingle(s => s == CrudEventType.Update);
            sizesOfFirstSelection.TryGetValue(CrudEventType.Update, out var updatedSizesOfFirstSelection)
                .Should().BeTrue();

            updatedSizesOfFirstSelection.First().Count.Should().Be(20);
            
            secondSelectionProductChanges.TryGetValue(CrudEventType.Deleted, out var secondSelectionChangedProducts).Should()
                .BeTrue();
            secondSelectionChangedProducts.First().Sizes.TryGetValue(CrudEventType.Deleted, out var deletedSizes)
                .Should().BeTrue();
            deletedSizes.Should().HaveCount(2, "When a product is deleted so are its sizes");


            changes.NewState.ProcurementKeySets.Should().BeEmpty();
            changes.NewState.SalePoints.Should().BeEmpty();
        }   
    }
}