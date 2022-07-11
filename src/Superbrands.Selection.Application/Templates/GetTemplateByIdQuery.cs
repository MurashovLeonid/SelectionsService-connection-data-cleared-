using MediatR;
using Superbrands.Selection.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Application.Templates
{
    public class GetTemplateByIdQuery : IRequest<NomenclatureTemplate>
    {
        public int TemplateId { get; }

        public GetTemplateByIdQuery(int templateId)
        {
            if (templateId <= 0)
                throw new ArgumentNullException(nameof(templateId));

            TemplateId = templateId;
        }
    }
}
