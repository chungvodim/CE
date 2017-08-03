using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BGP.Utils.Common
{
    public static class ExpressionExtension
    {
        ///// <summary>
        ///// Extension method to support dynamic sorting.
        ///// </summary>
        ///// <typeparam name="T">The entity type.</typeparam>
        ///// <param name="source">The entity list.</param>
        ///// <param name="statement">The order by statement.</param>
        ///// <returns></returns>
        //public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string statement)
        //{
        //    // NOTE: Currently only supports sorting of 1 column.
        //    string method = string.Empty;
        //    string[] parts = statement.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        //    // Determine sort order.
        //    method = (parts.Length > 1 && parts[1].ToUpper() == "DESC") ?
        //                "OrderByDescending" : "OrderBy";

        //    // Create dynamic expression.
        //    var type = typeof(T);
        //    var property = type.GetProperty(parts[0]);
        //    var parameter = Expression.Parameter(type, "param");
        //    var member = Expression.MakeMemberAccess(parameter, property);
        //    var lambda = Expression.Lambda(member, parameter);

        //    var finalExpression = Expression.Call(typeof(Queryable), method,
        //                            new Type[] { type, property.PropertyType },
        //                            source.Expression, Expression.Quote(lambda));

        //    return source.Provider.CreateQuery<T>(finalExpression);
        //}

        public static IQueryable<T> WhereMany<T>(this IQueryable<T> source, params Expression<Func<T, bool>>[] predicates)
        {
            if (predicates != null && predicates.Any())
            {
                foreach (var predicate in predicates)
                {
                    source = source.Where(predicate);
                }
            }
            return source;
        }

        public static IQueryable<T> WhereMany<T>(this IQueryable<T> source, IEnumerable<Expression<Func<T, bool>>> predicates)
        {
            return source.WhereMany(predicates.ToArray());
        }

        public static IList<T> CastToList<T>(this IEnumerable source)
        {
            return new List<T>(source.Cast<T>());
        }

        public static Expression<Func<T, bool>> Combine<T>(this IEnumerable<Expression<Func<T, bool>>> expressions)
        {

            if (expressions == null)
            {
                return t => true;
                //throw new ArgumentNullException("expressions");
            }
            if (expressions.Count() == 0)
            {
                return t => true;
            }
            Type delegateType = typeof(Func<,>)
                                    .GetGenericTypeDefinition()
                                    .MakeGenericType(new[] {
                                typeof(T),
                                typeof(bool)
                                    }
                                );
            var combined = expressions
                               .Cast<Expression>()
                               .Aggregate((e1, e2) => Expression.AndAlso(e1, e2));
            return (Expression<Func<T, bool>>)Expression.Lambda(delegateType, combined);
        }

    }
}
