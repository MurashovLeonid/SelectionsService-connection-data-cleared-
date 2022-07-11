using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Superbrands.Selection.Application.Selection
{
    public class ExportSelectionQuery : IRequest<FileContentResult>
    {
        public long SelectionId { get; }

        public ExportSelectionQuery(long selectionId)
        {
            if (selectionId <= 0) throw new ArgumentOutOfRangeException(nameof(selectionId));
            SelectionId = selectionId;
        }
    }
}