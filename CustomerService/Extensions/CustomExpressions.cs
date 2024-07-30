using CustomerService.DTO.Page;
using CustomerService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.OpenApi.Expressions;
using System;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
namespace CustomerService.Extensions
{
    public static class CustomExpressions
    {
        public static async Task<PageDTO<T>> ToPagedAsync<T>(this IQueryable<T> src
            , PagingOptions pagingOptions) where T : class
        {
            int skip = (pagingOptions.pageNumber - 1) * pagingOptions.pageSize;
            int take = pagingOptions.pageSize;
            string sortProperty = pagingOptions.SortProperty ;
            string sortDirection = pagingOptions.SortDirection ;
            var filters =pagingOptions.Filters;
            
            if (filters != null &&filters.Count() !=0)
            {
                var predicate = BuildPredicate<T>(filters);
                src = src.Where(predicate);
            }
            if (!string.IsNullOrWhiteSpace(sortProperty) && !string.IsNullOrWhiteSpace(sortDirection))
            {
                var property = sortProperty.Trim();
                src = sortDirection.Trim().Equals("desc", StringComparison.OrdinalIgnoreCase) ? src.OrderByDescending(property) : src.OrderBy(property);
            }

            var results = new PageDTO<T>
            {
                TotalItemCount = await src.CountAsync(),
                Items = await src.Skip(skip).Take(take).ToListAsync()
            };

            return results;
        }

        #region order by      
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
            var property = BuildPropertyPathExpression(parameter, propertyName);
            var convert = Expression.Convert(property, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(convert, parameter);
            return lambda;
        }
        #endregion
        #region Filter     
        public static Expression<Func<T, bool>> BuildPredicate<T>(List<FilterDTO> filters)
        {
            Expression combinedExpression = null;
            var parameter = Expression.Parameter(typeof(T), "x");
            foreach (var filter in filters)
            {
                var property = CustomExpressions.BuildPropertyPathExpression(parameter, filter.PropertyName);
                var propertyType = property.Type;
                Expression condition = null;

                if (propertyType == typeof(string))
                {
                    var value = Expression.Constant(filter.PropertyValue);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    condition = Expression.Call(property, containsMethod, value);
                    combinedExpression = combinedExpression==null ? condition
                        : Expression.AndAlso(combinedExpression, condition);
                }
                else
                {
                    object parsedValue = null;

                    if (TryParse(propertyType, filter.PropertyValue, out parsedValue))
                    {
                        var value = Expression.Constant(parsedValue, propertyType);
                        condition = Expression.Equal(property, value);
                        combinedExpression = combinedExpression == null ? condition
                        : Expression.AndAlso(combinedExpression, condition);
                    }
                    else
                    {
                        throw new ArgumentException($"Unable to parse '{filter.PropertyValue}' to {propertyType.Name}");
                    }
                }

            }

            return Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
        }

        private static bool TryParse(Type targetType, string value, out object result)
        {
            var tryParseMethod = targetType.GetMethod("TryParse", new[] { typeof(string), targetType.MakeByRefType() });
            if (tryParseMethod != null)
            {
                var parameters = new object[] { value, null };
                var success = (bool)tryParseMethod.Invoke(null, parameters);
                result = parameters[1];
                return success;
            }
            else
            {
                result = null;
                return false;
            }
        }
        #endregion
        #region Get Property   
        public static Expression BuildPropertyPathExpression(Expression rootExpression, string propertyName)
        {

            Expression currentExpression = rootExpression;
            var propertyInfo = currentExpression.Type.GetProperty(propertyName,
                System.Reflection.BindingFlags.IgnoreCase |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public);

            if (propertyInfo == null)
                throw new KeyNotFoundException($"Cannot find property {propertyName} on type {currentExpression.Type.Name}.");

            currentExpression = Expression.Property(currentExpression, propertyInfo);

            return currentExpression;
        }
        #endregion


        #region comments for expressions linq    
        #region filteration with one properity               

        //if (!string.IsNullOrWhiteSpace(searchProperty) && !string.IsNullOrWhiteSpace(searchValue))
        //{
        //    src = src.Where(ToLambdaFilter<T>(searchProperty, searchValue));
        //}
        #endregion
        //private static Expression FilterBy(Expression source,string filterName,string filterValue )
        //{
        //    source = Filter(source, filterName , filterValue);
        //    return source;
        //}
        //private static Expression Filter(Expression source, string filterName, string filterValue)
        //{
        //    var sourceType = source.Type.GetGenericArguments().First();
        //    var parameter = Expression.Parameter(sourceType, "x");
        //    var constant = Expression.Constant(filterValue);
        //    var property = Expression.Property(parameter, filterName);
        //    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        //    var filterValueExpression = Expression.Constant(filterValue, typeof(string));
        //    return Expression.Call(property, containsMethod, filterValueExpression);
        //}
        #region  Filter Region 
        //private static Expression<Func<T, bool>> ToLambdaFilter<T>(string propertyName, string filterValue) where T : class
        //{
        //var parameter = Expression.Parameter(typeof(T), "x");
        //var property = Expression.Property(parameter, propertyName);
        //var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        //var filterValueExpression = Expression.Constant(filterValue, typeof(string));
        //var containsExpression = Expression.Call(property, containsMethod, filterValueExpression);

        //return Expression.Lambda<Func<T, bool>>(containsExpression, parameter);
        //}
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
        #endregion


    }
}
