using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BGP.Utils.Common
{
    /// <summary>
    /// Enables the efficient, dynamic composition of query predicates.
    /// </summary>
    public static class PredicateBuilder
    {
        public const string ENTITY_NAME = "entity";

        /// <summary>
        /// Creates a predicate that evaluates to true.
        /// </summary>
        public static Expression<Func<T, bool>> True<T>() { return param => true; }

        /// <summary>
        /// Creates a predicate that evaluates to false.
        /// </summary>
        public static Expression<Func<T, bool>> False<T>() { return param => false; }

        /// <summary>
        /// Creates a predicate expression from the specified lambda expression.
        /// </summary>
        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate) { return predicate; }

        /// <summary>
        /// Combines the first predicate with the second using the logical "and".
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        /// Combines the first predicate with the second using the logical "or".
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>
        /// Negates the predicate.
        /// </summary>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }

        /// <summary>
        /// Combines the first expression with the second using the specified merge function.
        /// </summary>
        static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)
            var map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with the parameters in the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression CreateExpression<T>(string propertyName, string valueToCompare)
        {
            // get the type of entity
            var entityType = typeof(T);
            // get the type of the value object
            //var valueType = valueToCompare.GetType();
            var entityProperty = entityType.GetProperty(propertyName);
            var propertyType = entityProperty.PropertyType;


            // Expression: "entity"
            var parameter = Expression.Parameter(entityType, ENTITY_NAME);

            // check if the property type is a Enum type
            // only Enum types work 
            if (propertyType.IsEnum)
            {
                //var enumToCompare = Enum.Parse(propertyType, valueToCompare);
                var enumToCompare = Convert.ChangeType
                          (
                              Enum.Parse(propertyType, valueToCompare),
                              typeof(T).GetProperty(propertyName).PropertyType
                          );
                //Expression: entity.Property == value
                return Expression.Equal(
                    Expression.Property(parameter, entityProperty),
                    Expression.Constant(enumToCompare)
                );
            }
            // check if the property type is a value type
            // only value types work 
            else if (propertyType.IsValueType || propertyType.Equals(typeof(string)))
            {
                // Expression: entity.Property == value
                return Expression.Equal(
                    Expression.Property(parameter, entityProperty),
                    Expression.Constant(valueToCompare)
                );
            }
            // if not, then use the key
            else
            {
                // get the key property
                var keyProperty = propertyType.GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0);

                // Expression: entity.Property.Key == value.Key
                return Expression.Equal(
                    Expression.Property(
                        Expression.Property(parameter, entityProperty),
                        keyProperty
                    ),
                    Expression.Constant(
                        keyProperty.GetValue(valueToCompare),
                        keyProperty.PropertyType
                    )
                );
            }
        }

        public static Expression<Func<T, bool>> CreateLambdaExpression<T>(string propertyName, string valueToCompare)
        {
            var argument = Expression.Parameter(typeof(T), ENTITY_NAME);
            var expression = CreateExpression<T>(propertyName, valueToCompare);
            var predicate = Expression.Lambda<Func<T, bool>>(expression,argument);
            return predicate;
        }

        class ParameterRebinder : System.Linq.Expressions.ExpressionVisitor
        {
            readonly Dictionary<ParameterExpression, ParameterExpression> map;

            ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                ParameterExpression replacement;

                if (map.TryGetValue(p, out replacement))
                {
                    p = replacement;
                }

                return base.VisitParameter(p);
            }
        }
    }
}
