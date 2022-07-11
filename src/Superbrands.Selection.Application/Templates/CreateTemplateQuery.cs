using MediatR;
using Superbrands.Selection.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Application.Templates
{
    public class CreateTemplateQuery : IRequest<NomenclatureTemplate>
    {
        public NomenclatureTemplate NomenclatureTemplate { get; }

        public CreateTemplateQuery(NomenclatureTemplate nomenclatureTemplate)
        {
            NomenclatureTemplate = nomenclatureTemplate ?? throw new ArgumentNullException(nameof(nomenclatureTemplate));
        }
    }
}
