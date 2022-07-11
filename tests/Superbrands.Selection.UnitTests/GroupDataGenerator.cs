using AutoFixture.Xunit2;
using Newtonsoft.Json;
using Superbrands.Selection.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Superbrands.Selection.Domain.Selections;
using Xunit;

namespace Superbrands.Selection.UnitTests
{
    public class GroupDataGenerator
    {

        [Theory]
        [AutoData]
        public async Task CreateProducts_5()
        {
            var colorModels = GetColorModels(5);
            var json = JsonConvert.SerializeObject(colorModels);
        }

        public static List<ColorModelMeta> GetColorModels(int count)
        {
            var sizes = new List<Superbrands.Selection.Domain.Size>
            {
                new("SB19-878233.42", 15, 300, 210),
                new("SB19-878234.43", 10, 400, 300),
                new("SB19-878235.44", 25, 500, 400)
            };

            var sizes2 = new List<Superbrands.Selection.Domain.Size>
            {
                new("SB19-878253.42", 15, 300, 220),
                new("SB19-878254.43", 10, 400, 300),
                new("SB19-878255.44", 25, 500, 400)
            };

            var sizesDict = new Dictionary<int, List<Superbrands.Selection.Domain.Size>> {{0, sizes}, {1, sizes2}};


            var colorModels = new List<ColorModelMeta>();
            var random = new Random();

            for (int i = 0; i < count; i++)
                colorModels.Add(new ColorModelMeta(random.Next(1, 100).ToString(), random.Next(1, 100),
                    random.Next(1, 100).ToString(), ColorModelStatus.None, ColorModelPriority.PriorityA,
                    sizesDict[random.Next(0, 1)], "$"));

            return colorModels;
        }
    }
}
