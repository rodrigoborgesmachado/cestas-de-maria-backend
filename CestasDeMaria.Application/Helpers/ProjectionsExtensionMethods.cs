using CestasDeMaria.Infrastructure.CrossCutting.Adapter;

namespace CestasDeMaria.Application.Helpers
{
    public static class ProjectionsExtensionMethods
    {
        /// <summary>
        /// Project a type using a DTO
        /// </summary>
        /// <typeparam name="TProjection">The dto projection</typeparam>
        /// <param name="entity">The source entity to project</param>
        /// <returns>The projected type</returns>
        public static TProjection ProjectedAs<TProjection>(this object item)
            where TProjection : class, new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<TProjection>(item);
        }

        /// <summary>
        /// projected a enumerable collection of items
        /// </summary>
        /// <typeparam name="TProjection">The dtop projection type</typeparam>
        /// <param name="items">the collection of entity items</param>
        /// <returns>Projected collection</returns>
        public static IEnumerable<TProjection> ProjectedAsCollection<TProjection>(this IEnumerable<object> items)
            where TProjection : class, new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<IEnumerable<TProjection>>(items);
        }
    }
}
