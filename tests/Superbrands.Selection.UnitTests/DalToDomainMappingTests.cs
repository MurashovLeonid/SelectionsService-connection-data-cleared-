using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using KellermanSoftware.CompareNetObjects;
using Superbrands.Selection.Bus;
using Superbrands.Selection.Domain.Events;
using Superbrands.Selection.Domain.Procurements;
using Superbrands.Selection.Infrastructure.DAL;
using Xunit;

namespace Superbrands.Selection.UnitTests
{
    public class DalToDomainMappingTests : TestBase
    {
        [Fact]
        public void SelectionDalToDomain()
        {
            var selectionDalDto = _fixture.Create<SelectionDalDto>();

            var domain = selectionDalDto.ToDomain();
            var comparisonResult = _compareLogic.Compare(selectionDalDto, domain);
            
            if (comparisonResult.Differences.Any())
                throw new Exception(comparisonResult.DifferencesString);
        }
    }
}