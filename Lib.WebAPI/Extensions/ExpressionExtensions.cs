using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Lib.WebAPI.Business;

namespace Lib.WebAPI.Extensions
{
    /// <summary>
    /// ExpressionExtensions
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Ands the also.
        /// </summary>
        /// <param name="expr1">The expr1.</param>
        /// <param name="expr2">The expr2.</param>
        public static Expression<Func<T, bool>> AndAlso<T>(
                this Expression<Func<T, bool>> expr1,
                Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(left!, right!), parameter);
        }
    }
}