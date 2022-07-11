using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Moq;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Domain.Procurements;
using Superbrands.Selection.Domain.SalePoints;
using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.UnitTests.Mocks
{
    internal static class ProcurementDomainStub
    {
        public static Procurement GetOne()
        {
            var salePoints = new List<SalePoint>
            {
                new(1, true),
                new(2, true)
            };
            var brandsDiscounts = new Dictionary<long, decimal>
            {
                {1, 1},
                {2, 2}
            };

            var selection1 = new Selection.Domain.Selections.Selection(1, 1, SelectionStatus.InProgress) {Id = 1};
            var products = GetMeta(2, 1);
            foreach (var product in products) selection1.AddProduct(product);

            var selection2 = new Selection.Domain.Selections.Selection(1, 1, SelectionStatus.InProgress) {Id = 2};
            products = GetMeta(2, 2);
            foreach (var product in products) selection2.AddProduct(product);

            var proc = new Procurement(Guid.NewGuid().ToString(), 1, 1, ProcurementKind.Wholesale,
                ProcurementStage.FreeWarehouse, ProcurementStatus.Selection, salePoints, brandsDiscounts, new List<CounterpartyCondition>(),1, false) {Id = 1};

            proc.AddSelection(selection1);
            proc.AddSelection(selection2);
            proc.ProcurementKeySets = new List<ProcurementKeySet>(new List<ProcurementKeySet>
            {
                new(1, 1, 1, 1),
                new(2, 2, 2, 2)
            });
            proc.AddBrands(new List<long> {1, 2});

            return proc;
        }

        /// <summary>
        /// изменено:
        /// кол-во товара (1) в 1 отборке и изменен ее статус
        /// удален товар во второй отборке
        /// </summary>
        /// <returns></returns>
        public static Procurement GetModified()
        {
            var proc = GetOne();
            var firstSelection = proc.Selections.First();
            var firstProductOfFirstSelection = firstSelection.ColorModelMetas.First();
            var sizesOfFirstSelection = firstProductOfFirstSelection.Sizes;
            var firstSizeOfFirstSelection = sizesOfFirstSelection.First();
            firstSizeOfFirstSelection.Count = 20; // в первом товаре первой отборки стало 20 штук (Update)

            var secondSelection = proc.Selections.Skip(1).First();
            secondSelection.RemoveProduct("abc1"); //Во второй отборке больше нет товаров (Delete)

            return proc;
        }

        private static IEnumerable<ColorModelMeta> GetMeta(int number, long selectionId)
        {
            for (var i = 1; i < number; i++)
            {
                yield return new($"abc{i}", selectionId, $"bcd{i}",
                    ColorModelStatus.New, ColorModelPriority.PriorityA, "$")
                {
                    Id = i,
                    Sizes = new List<Size>
                    {
                        new($"size{i}", number, 1, 1),
                        new($"size-1-{i}", number, number, number)
                    }
                };
            }
        }
    }
}