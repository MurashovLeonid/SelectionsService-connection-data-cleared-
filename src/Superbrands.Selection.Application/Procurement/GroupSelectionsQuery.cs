using MediatR;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Infrastructure.RepositoryResponses;
using System;
using System.Collections.Generic;
using System.Text;
using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.Application.Procurement
{
    public class GroupSelectionsQuery : IRequest<IEnumerable<SelectionGroup>>
    {
        public GroupKeyType GroupKeyType { get;  }
        public BTSelectionsGroupResponse ResponseFromBt { get;  }
        public int ProcurementId { get;  }

        public GroupSelectionsQuery(int procurementId, BTSelectionsGroupResponse responseFromBt, GroupKeyType groupKeyType)
        {
            ResponseFromBt = responseFromBt ?? throw new ArgumentNullException(nameof(responseFromBt));
            GroupKeyType = groupKeyType;
            ProcurementId = procurementId;
        }

    }
}
