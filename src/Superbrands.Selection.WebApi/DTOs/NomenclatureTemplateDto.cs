using Superbrands.Selection.Domain;
using System.Collections.Generic;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class NomenclatureTemplateDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<int> Parameters { get; set; }

        public NomenclatureTemplateDto(NomenclatureTemplate nomenclatureTemplate)
        {
            Id = nomenclatureTemplate.Id;
            Name = nomenclatureTemplate.Name;
            Parameters = nomenclatureTemplate.Parameters;
        }
        public NomenclatureTemplate ToDomain()
        {
            return new NomenclatureTemplate(Name, Parameters);
        }
    }
}
