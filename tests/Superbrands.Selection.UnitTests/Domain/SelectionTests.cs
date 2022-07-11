using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Domain.Events;
using Superbrands.Selection.Domain.Selections;
using Xunit;

#pragma warning disable 1998

namespace Superbrands.Selection.UnitTests.Domain
{
    public class SelectionTests
    {
        [Theory, AutoData]
        public async Task Create_Created(long clientId, int procurementId)
        {
            var selection = new Selection.Domain.Selections.Selection(clientId, procurementId, SelectionStatus.Unset);

            selection.Should().NotBeNull();

            selection.Id.Should().Be(0, "for new selection");
        }

        [Theory, AutoData]
        public async Task AddProducts_ProductsAdded(string modelVendorCodeSbs, string colormodelVendorCodeSbs, int selectionId, long clientId,
            long partnerId, int procurementId)
        {
            var selection = new Selection.Domain.Selections.Selection(clientId, procurementId, SelectionStatus.Unset);

            var meta = new ColorModelMeta(modelVendorCodeSbs, selectionId, colormodelVendorCodeSbs, ColorModelStatus.None,
                ColorModelPriority.PriorityA,"$");
            selection.AddProduct(meta);

            selection.ColorModelMetas.Should().HaveCount(1);
        }

        [Theory, AutoData]
        public void SetStatus_StatusSet(long clientId, int procurementId)
        {
            var selection = new Selection.Domain.Selections.Selection(clientId, procurementId, SelectionStatus.Unset);
            var newStatus = SelectionStatus.InProgress;
            selection.SetStatus(newStatus);

            selection.Status.Should().BeEquivalentTo(newStatus);
        }

        [Theory, AutoData]
        public void SetStatusBackwards_Works(long clientId, int procurementId)
        {
            var selection = new Selection.Domain.Selections.Selection(clientId, procurementId, SelectionStatus.Unset);
            selection.SetStatus(SelectionStatus.Agreed);

            selection.SetStatus(SelectionStatus.InProgress);

            selection.Status.Should().Be(SelectionStatus.InProgress);
        }
    }
}