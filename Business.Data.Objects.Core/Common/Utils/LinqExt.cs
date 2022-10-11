using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Business.Data.Objects.Common.Utils
{

    /// <summary>
    /// Metodi di estensione per espressioni linq
    /// </summary>
    internal static class LinqExt
    {

        /// <summary>
        /// Ritorna espressione combinata in AND
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        internal static Expression<Func<T, bool>> AndAlso<T>(Expression<Func<T, bool>> exp, Expression<Func<T, bool>> other)
        {
            //var a = Expression.AndAlso(expression, other);
            //return (Expression<Func<T, bool>>)(Expression.Lambda(a, null));

            Expression<Func<T, Boolean>> combined = Expression.Lambda<Func<T, Boolean>>(
                                        Expression.AndAlso(
                                            exp.Body,
                                            new ExpressionParameterReplacer(other.Parameters, exp.Parameters).Visit(other.Body)
                                            ), exp.Parameters);

            return combined;

        }

        /// <summary>
        /// Ritorn espressione combinata in OR
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        internal static Expression<Func<T, bool>> OrElse<T>(Expression<Func<T, bool>> exp, Expression<Func<T, bool>> other)
        {
            //var a = Expression.OrElse(expression, other);
            //return (Expression<Func<T, bool>>)(Expression.Lambda(a, null));

            Expression<Func<T, Boolean>> combined = Expression.Lambda<Func<T, Boolean>>(
                            Expression.OrElse(
                                exp.Body,
                                new ExpressionParameterReplacer(other.Parameters, exp.Parameters).Visit(other.Body)
                                ), exp.Parameters);

            return combined;
        }

    }

    internal class ExpressionParameterReplacer : ExpressionVisitor
    {
        private IDictionary<ParameterExpression, ParameterExpression> ParameterReplacements { get; set; }

        public ExpressionParameterReplacer
        (IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters)
        {
            ParameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();

            for (int i = 0; i != fromParameters.Count && i != toParameters.Count; i++)
            { ParameterReplacements.Add(fromParameters[i], toParameters[i]); }
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            ParameterExpression replacement;

            if (ParameterReplacements.TryGetValue(node, out replacement))
            { node = replacement; }

            return base.VisitParameter(node);
        }
    }

}