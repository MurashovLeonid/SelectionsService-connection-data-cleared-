using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using Mapster;
using Superbrands.Libs.RestClients.Pim;
using Superbrands.Selection.Domain.Procurements;

namespace Superbrands.Selection.Infrastructure.DAL
{
    public class SelectionDalDto : AuditableEntity
    {
        public SelectionStatus Status { get; set; } = SelectionStatus.InProgress;
        public long BuyerId { get; set; }
        public ICollection<SelectionPurchaseSalePointKeyDalDto> SelectionPurchaseSalePointKeys { get; set; } = new List<SelectionPurchaseSalePointKeyDalDto>();
        public long ProcurementId { get; set; }
        public virtual ProcurementDalDto Procurement { get; set; }
        public ICollection<ColorModelMetaDalDto> ColorModelMetas { get; set; } = new List<ColorModelMetaDalDto>();
        public IEnumerable<LogEntryDalDto> Logs { get; set; }

        public Domain.Selections.Selection ToDomain(Procurement procurement)
        {
            var sel = new Domain.Selections.Selection(BuyerId, ProcurementId, Status,
                SelectionPurchaseSalePointKeys?.Select(x => x.ToDomain()).ToList(),
                ColorModelMetas?.Select(x => x.ToDomain()).ToList()) {Id = Id};
            sel.SetProcurement(procurement);
            sel.Procurement._selections = new();
            sel.Procurement._selections.Add(sel);
            return sel;
        }

        public Domain.Selections.Selection ToDomain()
        {
            return this.Adapt<Domain.Selections.Selection>();
        }

        public static SelectionDalDto FromDomain(Domain.Selections.Selection selection)
        {
            var dalDto = selection.Adapt<SelectionDalDto>();
            dalDto.ColorModelMetas = selection.ColorModelMetas.Where(c => !c.Removed)
                .Select(c => c.Adapt<ColorModelMetaDalDto>()).ToList();
            return dalDto;
        }
    }
}