using CustomerService.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace CustomerService.Extensions
{
    public static class CustomExpressions
    {
        #region  order by property name and direction whith filtering and pagination
        public static async Task<PageDTO<T>> ToPagedAsync<T>(this IQueryable<T> src
            , int skip, int take
            , string sortProperty = null, string sortDirection = null
            , string filterProperty = null, string filterValue = null) where T : class
        {
            if (!string.IsNullOrWhiteSpace(filterProperty) && !string.IsNullOrWhiteSpace(filterValue))
            {
                src = src.Where(ToLambdaFilter<T>(filterProperty, filterValue));
            }
            if (!string.IsNullOrWhiteSpace(sortProperty) && !string.IsNullOrWhiteSpace(sortDirection))
            {
                var property = sortProperty;
                src = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) ? src.OrderByDescending(property) : src.OrderBy(property);
            }
            var results = new PageDTO<T>
            {
                TotalItemCount = await src.CountAsync(),
                Items = await src.Skip(skip).Take(take).ToListAsync()
            };

            return results;
        }

        #region  Filter Region 
        private static Expression<Func<T, bool>> ToLambdaFilter<T>(string propertyName, string filterValue) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var filterValueExpression = Expression.Constant(filterValue, typeof(string));
            var containsExpression = Expression.Call(property, containsMethod, filterValueExpression);

            return Expression.Lambda<Func<T, bool>>(containsExpression, parameter);
        }
        #endregion

        #region Order Region 
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderBy(ToLambda<T>(propertyName));
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderByDescending(ToLambda<T>(propertyName));
        }

        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda<Func<T, object>>(property, parameter);

            return lambda;
        }
        #endregion

        #endregion

        //#region sort by property . then by property and paginate
        //public static async Task<PageDTO<T>> ToPagedAsync<T>(this IQueryable<T> src, int skip, int take, string orderBy = null) where T : class
        //{
        //    var queryExpression = src.Expression;
        //    queryExpression = queryExpression.OrderBy(orderBy);

        //    if (queryExpression.CanReduce)
        //        queryExpression = queryExpression.Reduce();

        //    src = src.Provider.CreateQuery<T>(queryExpression);

        //    var results = new PageDTO<T>
        //    {
        //        TotalItemCount = await src.CountAsync(),
        //        Items = await src.Skip(skip).Take(take).ToListAsync()
        //    };

        //    return results;
        //}

        //private static Expression OrderBy(this Expression source, string orderBy)
        //{
        //    if (!string.IsNullOrWhiteSpace(orderBy))
        //    {
        //        var orderBys = orderBy.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //        for (int i = 0; i < orderBys.Length; i++)
        //        {
        //            source = AddOrderBy(source, orderBys[i], i);
        //        }
        //    }

        //    return source;
        //}

        //private static Expression AddOrderBy(Expression source, string orderBy, int index)
        //{
        //    //colimn name and direction
        //    var orderByParams = orderBy.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //    string orderByMethodName = index == 0 ? "OrderBy" : "ThenBy";
        //    //column name
        //    string parameterPath = orderByParams[0];
        //    //check if is desc or normal ordering
        //    if (orderByParams.Length > 1 && orderByParams[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
        //        orderByMethodName += "Descending";

        //    var sourceType = source.Type.GetGenericArguments().First();
        //    var parameterExpression = Expression.Parameter(sourceType, "p");
        //    var orderByExpression = BuildPropertyPathExpression(parameterExpression, parameterPath);
        //    var orderByFuncType = typeof(Func<,>).MakeGenericType(sourceType, orderByExpression.Type);
        //    var orderByLambda = Expression.Lambda(orderByFuncType, orderByExpression, new ParameterExpression[] { parameterExpression });

        //    source = Expression.Call(typeof(Queryable), orderByMethodName, new Type[] { sourceType, orderByExpression.Type }, source, orderByLambda);
        //    return source;
        //}

        //private static Expression BuildPropertyPathExpression(this Expression rootExpression, string propertyPath)
        //{
        //    var parts = propertyPath.Split(new[] { '.' }, 2);
        //    var currentProperty = parts[0];
        //    var propertyDescription = rootExpression.Type.GetProperty(currentProperty, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        //    if (propertyDescription == null)
        //        throw new KeyNotFoundException($"Cannot find property {rootExpression.Type.Name}.{currentProperty}. The root expression is {rootExpression} and the full path would be {propertyPath}.");
        //    //rootExpression is the object that contain the properity "p" which is alias of "class" here.
        //    var propExpr = Expression.Property(rootExpression, propertyDescription);
        //    if (parts.Length > 1)
        //        return BuildPropertyPathExpression(propExpr, parts[1]);

        //    return propExpr;
        //}

        //#endregion

    }
}
