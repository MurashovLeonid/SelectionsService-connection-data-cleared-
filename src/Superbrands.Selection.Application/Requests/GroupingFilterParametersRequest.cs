
using Superbrands.Selection.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Superbrands.Libs.RestClients.Pim;

namespace Superbrands.Selection.Application.Requests
{
    public class GroupingFilterParametersRequest : SearchProductsRequest
    {
        public int GroupKeyId { get; set; }
        public GroupKeyType GroupKeyType { get; set; }
    }
}
