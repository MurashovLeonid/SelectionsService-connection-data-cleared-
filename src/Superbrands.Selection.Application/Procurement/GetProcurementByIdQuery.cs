using MediatR;
using System;
using System.Text;

namespace Superbrands.Selection.Application.Procurement
{
    public class GetProcurementByIdQuery : IRequest<Domain.Procurements.Procurement>
    {
        public long Id { get; }
        public GetProcurementByIdQuery(long id)
        {
            if (id <= 0)
                throw new ArgumentException(nameof(id));

            Id = id;
        }
    }
}
