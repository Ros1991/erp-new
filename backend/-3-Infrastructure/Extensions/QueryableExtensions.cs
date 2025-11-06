using System.Linq.Expressions;

namespace CAU.Infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Aplica ordenação dinâmica baseada em nome de propriedade
        /// </summary>
        public static IQueryable<T> OrderByProperty<T>(
            this IQueryable<T> source, 
            string propertyName, 
            bool ascending = true)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return source;

            // Validar se a propriedade existe (case-insensitive)
            var property = typeof(T).GetProperties()
                .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

            if (property == null)
                return source; // Retorna query original se propriedade não existir

            // Construir expressão: x => x.PropertyName
            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            // Chamar OrderBy ou OrderByDescending
            var methodName = ascending ? "OrderBy" : "OrderByDescending";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), property.PropertyType },
                source.Expression,
                Expression.Quote(lambda));

            return source.Provider.CreateQuery<T>(resultExpression);
        }
    }
}
