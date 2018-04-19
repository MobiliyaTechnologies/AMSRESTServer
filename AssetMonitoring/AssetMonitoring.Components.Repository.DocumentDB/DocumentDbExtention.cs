namespace AssetMonitoring.Components.Repository.DocumentDB
{
    using System.Linq;
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts.DocumentDbContract;
    using Microsoft.Azure.Documents.Linq;

    public static class DocumentDbExtention
    {
        public static async Task<DocumentDbPaginationResult<T>> PaginateDocument<T>(this IQueryable<T> query)
        {
            var documentQuery = query.AsDocumentQuery();

            var documentDbPaginationResult = new DocumentDbPaginationResult<T>();

            var result = await documentQuery.ExecuteNextAsync<T>();

            documentDbPaginationResult.ResponseContinuation = result.ResponseContinuation;
            documentDbPaginationResult.Result.AddRange(result);

            return documentDbPaginationResult;
        }
    }
}
