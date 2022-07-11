using System;
using System.Linq;
using AutoFixture;
using Mapster;
using Superbrands.Selection.Infrastructure.DAL;
using Xunit;

namespace Superbrands.Selection.UnitTests
{
    public class DomainToDalMappingsTests : TestBase
    {
        [Fact]
        public void Map_Selection_ToDal()
        {
            var domain = _fixture.Create<Selection.Domain.Selections.Selection>();

            var dal = domain.Adapt<SelectionDalDto>();
            var comparisonResult = _compareLogic.Compare(domain, 
                dal);
            
            if (comparisonResult.Differences.Any())
                throw new Exception(comparisonResult.DifferencesString);
        }
        
        [Fact(Skip = "нестабилен")]
        public void Map_Procurement_ToDal()
        {
            var domain = _fixture.Create<Selection.Domain.Procurements.Procurement>();

            var dal = domain.Adapt<ProcurementDalDto>();
            var comparisonResult = _compareLogic.Compare(domain, 
                dal);
            
            if (comparisonResult.Differences.Any())
                throw new Exception(comparisonResult.DifferencesString);
        }
    }
}