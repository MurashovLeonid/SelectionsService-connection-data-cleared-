using MediatR;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Superbrands.Selection.Application.Templates
{
    public class GetAllTemplatesQuery : IRequest<List<NomenclatureTemplate>>
    {
    }
}
