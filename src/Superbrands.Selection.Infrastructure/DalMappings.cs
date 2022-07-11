using System.Runtime.CompilerServices;
using Mapster;
using Superbrands.Libs.DDD.EfCore.Extensions;
using Superbrands.Selection.Domain.Procurements;
using Superbrands.Selection.Domain.Selections;
using Superbrands.Selection.Infrastructure.DAL;
[assembly: InternalsVisibleTo("Superbrands.Selection.UnitTests")]
namespace Superbrands.Selection.Infrastructure
{
    internal static class DalMappings
    {
        public static void Configure()
        {
            TypeAdapterConfig<ProcurementKeySetDalDto, ProcurementKeySet>.NewConfig().ConstructUsing(pks => new
                ProcurementKeySet(pks.ProcurementId, pks.BuyerId, pks.PurchaseKeyId, pks.FinancialPlaningCenterId));

            TypeAdapterConfig<ProcurementDalDto, Procurement>.NewConfig().ConstructUsing(dto => new Procurement(false))
                .Map(s => s._selections, s => s.Selections);

            TypeAdapterConfig<SelectionDalDto, Domain.Selections.Selection>.NewConfig()
                .ConstructUsing(s => new Domain.Selections.Selection())
                .Map(s => s.selectionPurchaseSalePointKeys, f => f.SelectionPurchaseSalePointKeys)
                .Map(s => s.colorModelMetas, d => d.ColorModelMetas)
                .Ignore(s => s.SalePointPriorityInfoCollection);

            TypeAdapterConfig<ColorModelMetaDalDto, ColorModelMeta>.NewConfig()
                .ConstructUsing(s => new(s.ModelVendorCodeSbs, s.SelectionId, s.ColorModelVendorCodeSbs, s.ColorModelStatus, s
                    .ColorModelPriority, s.Sizes, s.Currency));

            TypeAdapterConfig<SelectionPurchaseSalePointKeyDalDto, SelectionPurchaseSalePointKey>.NewConfig()
                  .ConstructUsing(s => new SelectionPurchaseSalePointKey(s.SelectionId, s.SalePointId, s.PurchaseKeyId));

            TypeAdapterConfig<ColorModelMeta, Bus.Contracts.CSharp.MsSelections.Selections.ColorModelMeta>
                 .NewConfig()
                 .Ignore(s => s.Sizes);

        }
    }
}