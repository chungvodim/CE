using System;
using System.Linq.Expressions;

namespace BGP.Utils.Repository
{
    public class PagingParams<T> 
    {
        public static int DefaultPageSize = 10;
        public bool IsAscending { get; set; }
        public string SortField { get; set; }
        public Expression<Func<T, object>> SortBy { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public int StartingIndex { get { return PageSize * Page; } }
        public Expression<Func<T, bool>> Predicate { get; set; }

        public PagingParams(Expression<Func<T, bool>> predicate = null)
        {
            SortField = "Id";
            IsAscending = true;
            PageSize = DefaultPageSize;
            Page = 0;
            Predicate = predicate;
        }
    }
}
