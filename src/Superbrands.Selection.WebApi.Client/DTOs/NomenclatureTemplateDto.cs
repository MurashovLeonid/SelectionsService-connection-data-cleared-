using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.WebApi.Client.DTOs
{
    public class NomenclatureTemplateDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<int> Parameters { get; set; }
    }
}
