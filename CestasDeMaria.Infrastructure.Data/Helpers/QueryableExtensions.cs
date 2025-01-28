using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CestasDeMaria.Infrastructure.Data.Helpers
{
    public static class QueryableExtensions
    {
        public static IEnumerable<T> ToListPaged<T>(this IQueryable<T> query, int page, int quantity)
        {
            if (page <= 0)
                page = 1;

            var skip = (page - 1) * quantity;

            return query.Skip(skip).Take(quantity).ToList();
        }

        public static async Task<IEnumerable<T>> ToListPagedAsync<T>(this IQueryable<T> query, int page, int quantity)
        {
            if (page <= 0)
                page = 1;

            var skip = (page - 1) * quantity;

            return await query.Skip(skip).Take(quantity).ToListAsync();
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string field, string direction, bool thenBy = false)
        {
            if (!string.IsNullOrEmpty(field) && !string.IsNullOrEmpty(direction))
            {
                MemberExpression property = null;
                ParameterExpression parameter = Expression.Parameter(query.ElementType, String.Empty);

                if (string.IsNullOrEmpty(field))
                {
                    field = parameter.Type.GetProperties().First().Name;
                }

                foreach (var propriedade in field.Split('.'))
                {
                    if (property == null)
                        property = Expression.Property(parameter, propriedade);
                    else
                        property = Expression.Property(property, propriedade);
                }

                LambdaExpression lambda = Expression.Lambda(property, parameter);

                string methodName = (direction.ToLower() == "desc") ? "OrderByDescending" : "OrderBy";

                if (thenBy)
                {
                    methodName = (direction.ToLower() == "desc") ? "ThenByDescending" : "ThenBy";
                }

                Expression methodCallExpression = Expression.Call(typeof(Queryable), methodName, new Type[] { query.ElementType, property.Type }, query.Expression, Expression.Quote(lambda));

                query = query.Provider.CreateQuery<T>(methodCallExpression);
            }

            return query;
        }
    }
}
