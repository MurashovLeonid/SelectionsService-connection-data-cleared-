using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Application.Options
{
    class ExportOptions
    {
        public List<AttributeColumn> Attributes { get; set; }
    }

    class AttributeColumn
    {
        public int AttributeId { get; set; }
        public HashSet<int> CompositeAttributes { get; set; }
        public bool IsCommon { get; set; }
        public bool IsComputed { get; set; }
        public string Name { get; set; }
    }
}