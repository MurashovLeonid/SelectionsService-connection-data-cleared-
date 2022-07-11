using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Domain
{
    public class PagedResult<T> 
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public long RowCount { get; set; }
        public int FirstRowOnPage { get; }
        public int LastRowOnPage { get; }

        public IList<T> Results { get; set; }
    }
}
