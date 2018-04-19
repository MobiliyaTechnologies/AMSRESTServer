namespace AssetMonitoring.Components.Repository
{
    using System.Linq;
    using AssetMonitoring.Components.Domain;

    public interface IRepository
    {
        /// <summary>
        /// Gets the specified entity, by id, immediately.
        /// </summary>
        /// <remarks>
        /// This is the recommended path to take when reading a single entity for purposes of showing or updating. It 
        /// is also the path to take when you want to recover from reading an entity that might not exist. If you need 
        /// to pull back a large number of sub-collections associated with the single entity, instead of taking the 
        /// lazy-load hit of traversing each collection, it might be best to use <see cref="Query"/> and projections 
        /// to load it all in one query.
        /// </remarks>
        /// <typeparam name="T">Type of the entity to get.</typeparam>
        /// <param name="entityId">The id of the entity.</param>
        /// <returns>Fully-loaded entity if exists; otherwise, <c>null</c>.</returns>
        T Read<T>(int entityId)
            where T : Entity;

        /// <summary>
        /// Creates a deferred LINQ query against the underlying type collection.
        /// </summary>
        /// <typeparam name="T">Type of the entity to get.</typeparam>
        /// <returns>A deferred LINQ query.</returns>
        IQueryable<T> Query<T>()
            where T : Entity;

        /// <summary>
        /// Informs the underlying data store to keep track of the <paramref name="entity"/> at the end of the 
        /// session. 
        /// </summary>
        /// <remarks>
        /// Use of this method may not immediately save to the underlying data store; it only tells the underlying 
        /// data store to be aware of it.
        /// </remarks>
        /// <typeparam name="T">Type of the entity to persist.</typeparam>
        /// <param name="entity">The entity to persist.</param>
        void Persist<T>(T entity)
            where T : Entity;

        /// <summary>
        /// Informs the underlying data store to delete the <paramref name="entity"/> at the end of the session.
        /// </summary>
        /// <remarks>
        /// Use of this method may not immediately save to the underlying data store; it only tells the underlying 
        /// data store to be aware of it.
        /// </remarks>
        /// <typeparam name="T">Type of the entity to delete.</typeparam>
        /// <param name="entity">The entity to delete.</param>
        void Delete<T>(T entity)
            where T : Entity;

        /// <summary>
        /// Partially saves changes.
        /// Used it only to get id of newly created entity in same request.
        /// Changes not saved to database until it's committed.
        /// </summary>
        void Flush();
    }
}
