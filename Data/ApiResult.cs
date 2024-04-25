using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using EFCore.BulkExtensions;

namespace worldCitiesServer.Data
{
    public class ApiResult<T>
    {
        private ApiResult(List<T> data,
            int count,
            int pageIndex,
            int pageSize,
            string? sortColumn,
            string? sortOrder,
            string? filterColumn = null,
            string? filterQuery = null)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            SortColumn = sortColumn;
            SortOrder = sortOrder;
            FilterColumn = filterColumn;
            FilterQuery = filterQuery;
        }
        public static async Task<ApiResult<T>> CreateAsync(IQueryable<T> source,
            int pageIndex,
            int pageSize,
            string? sortColumn = null,
            string? sortOrder = null,
            string? filterColumn = null,
            string? filterQuery = null)
        {
            // adding the filtering functionality 
            if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterQuery)
             && IsValidProperty(filterColumn))
            {
                source = source.Where(string.Format("{0}.StartsWith(@0)", filterColumn), filterQuery);
            }
            // getting the total count
            var count = await source.CountAsync();
            if (!string.IsNullOrEmpty(sortColumn)
             && IsValidProperty(sortColumn))
            {
                sortOrder = !string.IsNullOrEmpty(sortOrder)
                && sortOrder.ToUpper() == "ASC"
                ? "ASC"
                : "DESC";
                source = source.OrderBy(string.Format("{0} {1}", sortColumn, sortOrder));
            }
            source = source
            .Skip(pageIndex * pageSize)
            .Take(pageSize);

            // install the package EFCore.BulkExtensions
             #if DEBUG
            // retrieve the SQL query (for debug purposes)
            var sql = source.ToParametrizedSql();
            // TODO: do something with the sql string
             #endif
            var data = await source.ToListAsync();
            return new ApiResult<T>(
             data,
             count,
             pageIndex,
             pageSize,
             sortColumn,
             sortOrder,
             filterColumn,
             filterQuery);

        }

        private static bool IsValidProperty(string propertyName, bool throwExceptionIfNotFound = true)
        {
            var prop = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null && throwExceptionIfNotFound)
                throw new NotSupportedException(
                string.Format(
                $"ERROR: Property '{propertyName}' does not exist.")
                 );
            return prop != null;

        }

        public List<T> Data { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
        public string? FilterColumn { get; set; }
        public string? FilterQuery { get; set; }
        public bool HasPreviousPage { get { return (PageIndex > 0); } }
        public bool HasNextPage { get { return ((PageIndex + 1) < TotalPages); } }
    }
}
