using System.Collections.Generic;
using System.Linq;
using Superbrands.Selection.Domain.Logs;
using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.WebApi.DTOs
{
    public record SelectionPurchaseSalePointKeyDto(long Id,  long SelectionId, long SalePointId, long PurchaseKeyId);
    public class SelectionDto
    {
        public long Id { get; set; }
        public Domain.Enums.SelectionStatus Status { get; set; }
        public long ClientId { get; set; }
        public ICollection<ColorModelMetaDto> ColorModelMetas { get; set; }
        public int ProductsCount { get; set; }
        public ICollection<SelectionPurchaseSalePointKeyDto> SelectionPurchaseDepartmentKeys { get; set; }
        public int ColorModelsCount { get; set; }
        public int SizesCount { get; set; }
        public long BuyerId { get; set; }

        public List<LogEntry> Logs { get; set; }
        
        public SelectionDto(Domain.Selections.Selection selection)
        {
            Id = selection.Id;
            Status = selection.Status;
            ClientId = selection.BuyerId; //todo надо убрать, но сначала с фронтами согласовать
            BuyerId = selection.BuyerId;
            ProductsCount = selection.ColorModelMetas.Select(p => p.ModelVendorCodeSbs).Distinct().Count();
            ColorModelsCount = selection.ColorModelMetas.Count;
            SizesCount = selection.ColorModelMetas.SelectMany(p => p.Sizes).Select(x=>x.Sku).Distinct().Count();
            SelectionPurchaseDepartmentKeys = selection.SelectionPurchaseSalePointKeys.Select(k => new SelectionPurchaseSalePointKeyDto(k.Id, k.SelectionId, k.SalePointId, k.PurchaseKeyId)).ToList();
            Logs = selection.Logs.ToList();
            ColorModelMetas = selection.ColorModelMetas.Select(x => new ColorModelMetaDto(x)).ToList();
        }
    }
}