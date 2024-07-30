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
            #region intial values 
            int skip = (pagingOptions.pageNumber - 1) * pagingOptions.pageSize;
            int take = pagingOptions.pageSize;
            string sortProperty = pagingOptions.SortProperty ;
            string sortDirection = pagingOptions.SortDirection ;
            string firstName = pagingOptions.Filters.FirstName??"";
            string lastName = pagingOptions.Filters.LastName ?? "";
            string phoneNumber = pagingOptions.Filters.PhoneNumber ?? "";
            var parameter = Expression.Parameter(typeof(T), "x");
            var queryExpression = src.Expression;
            
            #endregion
            Expression combinedExpression = null;
            if (!string.IsNullOrWhiteSpace(firstName))
            {
                
                var firstNameExpression = CreateFilterExpression("FirstName", firstName);
                combinedExpression = combinedExpression == null
                    ? firstNameExpression
                    : Expression.AndAlso(combinedExpression, firstNameExpression);
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                var lastNameExpression = CreateFilterExpression("LastName", lastName);
                combinedExpression = combinedExpression == null
                    ? lastNameExpression
                    : Expression.AndAlso(combinedExpression, lastNameExpression);
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                var phoneNumberExpression = CreateFilterExpression("PhoneNumber", phoneNumber);
                combinedExpression = combinedExpression == null
                    ? phoneNumberExpression
                    : Expression.AndAlso(combinedExpression, phoneNumberExpression);
            }
            #region  integer   

            //if (age.HasValue)
            //{
            //    var ageExpression = CreateFilterExpression("Age", age.Value); // Example integer property
            //    combinedExpression = combinedExpression == null
            //        ? ageExpression
            //        : Expression.AndAlso(combinedExpression, ageExpression);
            //} 
            #endregion
            Expression CreateFilterExpression(string propertyName, object filterValue)
            {
                var property = Expression.Property(parameter, propertyName);
                var filterValueExpression = Expression.Constant(filterValue);

                if (filterValue is string)
                {
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    return Expression.Call(property, containsMethod, filterValueExpression);
                }
                else if (filterValue is int)
                {
                    var comparison = Expression.Equal(property, filterValueExpression);
                    return comparison;
                }

                throw new NotSupportedException("Filter value type not supported.");
            }

            if (combinedExpression != null)
            {
                
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                src =src.Where(lambda);

            }

            #region sorting 
            if (!string.IsNullOrWhiteSpace(sortProperty) && !string.IsNullOrWhiteSpace(sortDirection))
            {
                var property = sortProperty.Trim();
                src = sortDirection.Trim().Equals("desc", StringComparison.OrdinalIgnoreCase) ? src.OrderByDescending(property) : src.OrderBy(property);
            }
            #endregion
            var results = new PageDTO<T>
            {
                TotalItemCount = await src.CountAsync(),
                Items = await src.Skip(skip).Take(take).ToListAsync()
            };

            return results;
        }
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
            var property = BuildPropertyPathExpression(parameter, propertyName);
            var convert = Expression.Convert(property, typeof(object)); 
            var lambda = Expression.Lambda<Func<T, object>>(convert, parameter);
            return lambda;
        }

        private static Expression BuildPropertyPathExpression(Expression rootExpression, string propertyName)
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
        //private static Expression<Func<T, object>> ToLambda<T>(this Expression source, string propertyName)
        //{
        //    try
        //    {
        //        var sourceType = source.Type.GetGenericArguments().First();
        //        var parameterExpression = Expression.Parameter(sourceType, "p");
        //        var property = Expression.Property(parameterExpression, propertyName);
        //        var orderByFuncType = typeof(Func<,>).MakeGenericType(sourceType, property.Type);
        //        var orderByLambda = Expression.Lambda(orderByFuncType, property, new ParameterExpression[] { parameterExpression });

        //        return orderByLambda;
        //    }
        //    catch (Exception ex) {
        //        throw new Exception();
        //    }

        //}
        #endregion

        #region demo 
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
