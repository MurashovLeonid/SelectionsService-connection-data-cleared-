using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Domain;
using System.Collections.Generic;
using Mapster;

namespace Superbrands.Selection.Infrastructure.DAL
{
    public class NomenclatureTemplateDalDto : AuditableEntity
    {
        public string Name { get; set; }
        public IEnumerable<int> Parameters { get; set; }
        public NomenclatureTemplate ToDomain()
        {
            return new(Name, Parameters) { Id = Id };
        }

        public static NomenclatureTemplateDalDto FromDomain(NomenclatureTemplate nomenclatureTemplate)
        {
            return nomenclatureTemplate.Adapt<NomenclatureTemplateDalDto>();
        }
    }
}
