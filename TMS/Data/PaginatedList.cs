using System;
using System.Collections.Generic;
using System.Linq;

namespace TMS.Data
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; set; }

        public int TotalPages { get; set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }
        
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static PaginatedList<T> Create(IList<T> source, int pageindex, int pageSize)
        {
            var count = source.Count;
            var items = source.Skip((pageindex - 1) * pageSize)
                .Take(pageSize).ToList();

            return new PaginatedList<T>(items, count, pageindex, pageSize);
        }
    }
}
