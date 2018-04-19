namespace AssetMonitoring.Components.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides document db related operations.
    /// </summary>
    public interface IDocumentDbRepository
    {
        /// <summary>
        /// Creates a deferred LINQ query against the underlying type collection.
        /// </summary>
        /// <typeparam name="T">Type of the entity to get.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="pageSize">Total number of records to query.</param>
        /// <returns>A deferred LINQ query.</returns>
        IQueryable<T> Query<T>(string collection, string partitionKey = null, string continuationToken = null, int pageSize = -1)
            where T : class;

        /// <summary>
        /// Creates a deferred LINQ query against the underlying type collection.
        /// </summary>
        /// <typeparam name="T">Type of the entity to get.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="query">The query.</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="pageSize">Total number of records to query.</param>
        /// <returns>
        /// A deferred LINQ query.
        /// </returns>
        IQueryable<T> GetDocumentByQuery<T>(string collection, string query, string partitionKey = null, string continuationToken = null, int pageSize = -1)
            where T : class;

        /// <summary>
        /// Initializes the documentdb  database.
        /// </summary>
        Task InitializeDocumentDB();

        /// <summary>
        /// Executes the stored procedure asynchronous.
        /// </summary>
        /// <typeparam name="T">Type of the entity to get.</typeparam>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="storeProcedureName">The store procedure name.</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="procedureParams">The procedure parameters.</param>
        /// <returns>The entity of type T.</returns>
        Task<T> ExecuteStoredProcedureAsync<T>(string collectionName, string storeProcedureName, string partitionKey = null, params dynamic[] procedureParams);
    }
}
