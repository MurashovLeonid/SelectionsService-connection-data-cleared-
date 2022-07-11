using System;

namespace Superbrands.Libs.WebApiPaged
{
    public class PagedRequest<T>
    {
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }
        public T Specification { get; set; }
    }
}