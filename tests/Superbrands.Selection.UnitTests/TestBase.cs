using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using KellermanSoftware.CompareNetObjects;
using Superbrands.Selection.Application;
using Superbrands.Selection.Bus;
using Superbrands.Selection.Infrastructure;

namespace Superbrands.Selection.UnitTests
{
    public abstract class TestBase
    {
        protected readonly Fixture _fixture;
        protected readonly CompareLogic _compareLogic;
        public TestBase()
        {
            AppMappings.Configure();
            BusMappings.Configure();
            DalMappings.Configure();
            _fixture = new Fixture();

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new NullRecursionBehavior());

            _compareLogic = new CompareLogic
            {
                Config =
                {
                    MaxDifferences = 20, CompareProperties = true, IgnoreObjectTypes = true,
                    MaxStructDepth = 5, MembersToIgnore = new List<string> {"ColorModelMetas"}
                },
            };
        }
    }
}