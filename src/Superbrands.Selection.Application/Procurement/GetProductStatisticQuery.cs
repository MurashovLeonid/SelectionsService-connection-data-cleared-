using MediatR;
using Superbrands.Selection.Domain.Selections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Superbrands.Selection.Application.Procurement
{
    public class GetProductStatisticQuery : IRequest<List<ColorModelMeta>>
    {
        public long ProcurementsId { get; }
        public List<string> ModelVendoreCodes { get; }

        public GetProductStatisticQuery(long procurementsId, List<string> modelVendoreCodes)
        {
            if (procurementsId <= 0)
                throw new ArgumentException(nameof(procurementsId));

            ProcurementsId = procurementsId;
            ModelVendoreCodes = modelVendoreCodes.Any() ? modelVendoreCodes : throw new ArgumentException(nameof(modelVendoreCodes));
        }
    }
}
