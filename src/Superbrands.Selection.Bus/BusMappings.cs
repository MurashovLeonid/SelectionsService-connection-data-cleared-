using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Mapster;
using Superbrands.Bus.Contracts.CSharp;
using Superbrands.Bus.Contracts.CSharp.MsSelections.Enums;
using Superbrands.Selection.Application.Procurement;
using Superbrands.Selection.Bus.DiffComparers;
using Superbrands.Selection.Domain.Events;
using Superbrands.Selection.Domain.Procurements;
using Superbrands.Selection.Domain.Selections;

[assembly: InternalsVisibleTo("Superbrands.Selection.UnitTests")]

namespace Superbrands.Selection.Bus
{
    internal static class BusMappings
    {
        public static void Configure()
        {
            TypeAdapterConfig<ColorModelMeta, Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.ColorModelMeta>.NewConfig()
                .ConstructUsing(x => new Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.ColorModelMeta
                {
                    Currency = x.Currency,
                    ActivityId = x.ColorModelGroupKeys.ActivityId,
                    BrandId = x.ColorModelGroupKeys.BrandId,
                    SelectionId = x.SelectionId,
                    ActivityTypeId = x.ColorModelGroupKeys.ActivityId,
                    AssortmentGroupId = (int) x.ColorModelGroupKeys.AssortmentGroupId,
                    ColorModelPriority = (ColorModelPriority) x.ColorModelPriority,
                    ColorModelStatus = (ColorModelStatus) x.ColorModelStatus,
                    PurchaseKeyId = (int) x.ColorModelGroupKeys.PurchaseKeyId,
                    SalePointId = x.ColorModelGroupKeys.SalePointId,
                    SizeChartCount = x.SizeChartCount,
                    SizeChartId = x.SizeChartId,
                    ModelVendorCodeSbs = x.ModelVendorCodeSbs,
                    ColorModelVendorCodeSbs = x.ColorModelVendorCodeSbs
                }).Ignore(c => c.Sizes);

            TypeAdapterConfig<Domain.Selections.Selection, Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.Selection>
                .NewConfig().Ignore(x => x.ColorModelMetas);

            TypeAdapterConfig<Procurement, Superbrands.Bus.Contracts.CSharp.MsSelections.Procurement.Procurement>
                .NewConfig()
                .Ignore(s => s.SalePoints)
                .Ignore(s => s.Selections)             
                .Ignore(s => s.ProcurementKeySets)
                .Map(p => p.IsApproved, p => p.IsApproved())
                .Map(s=>s.SeasonCapsuleId, f=>f.SeasonId.ToString());

            TypeAdapterConfig<Selection.Domain.Selections.Selection, Superbrands.Bus.Contracts.CSharp.MsSelections.Selections.Selection>
                 .NewConfig()
                 .Ignore(s => s.ColorModelMetas)
                 .Ignore(s => s.SelectionPurchaseSalePointKeys);
        }
    }
}