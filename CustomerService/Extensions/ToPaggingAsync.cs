using CustomerService.DTO.Page;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CustomerService.Extensions
{
    public static class ToPaggingAsync
    {
        public static async Task<PageDTO<T>> PageAsync<T>(this IQueryable<T> src
           , PagingOptions pagingOptions) where T : class
        {
            int skip = (pagingOptions.pageNumber - 1) * pagingOptions.pageSize;
            int take = pagingOptions.pageSize;
            string sortProperty = pagingOptions.SortProperty;
            string sortDirection = pagingOptions.SortDirection;
            if (!string.IsNullOrWhiteSpace(sortProperty) && !string.IsNullOrWhiteSpace(sortDirection))
            {
                var property = sortProperty.Trim();
                src = sortDirection.Trim().Equals("desc", StringComparison.OrdinalIgnoreCase) ? src.OrderByDesc(property) : src.OrderByAsc(property);
            }

            var results = new PageDTO<T>
            {
                TotalItemCount = await src.CountAsync(),
                Items = await src.Skip(skip).Take(take).ToListAsync()
            };

            return results;
        }
        public static IOrderedQueryable<T> OrderByAsc<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderBy(ToLambda<T>(propertyName));
        }

        public static IOrderedQueryable<T> OrderByDesc<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderByDescending(ToLambda<T>(propertyName));
        }

        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = BuildPropertyPathExpression(parameter, propertyName);
            var convert = Expression.Convert(property, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(convert, parameter);
            return lambda;
        }
        private static Expression BuildPropertyPathExpression(Expression rootExpression, string propertyName)
        {
            Expression currentExpression = rootExpression;
            propertyName = propertyName.Trim();
            if (propertyName.Length == 0)
                throw new KeyNotFoundException($"Empty {propertyName} on type {currentExpression.Type.Name} Not Allowed.");
            var propertyInfo = currentExpression.Type.GetProperty(propertyName,
                System.Reflection.BindingFlags.IgnoreCase |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public);

            if (propertyInfo == null)
                throw new KeyNotFoundException($"Cannot find property {propertyName} on type {currentExpression.Type.Name}.");

            currentExpression = Expression.Property(currentExpression, propertyInfo);

            return currentExpression;
        }
    }
}
