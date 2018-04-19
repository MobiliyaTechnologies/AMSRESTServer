namespace AssetMonitoring.DocumentDB.WebJob.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AssetMonitoring.Components.Repository;
    using AssetMonitoring.DocumentDB.WebJob.Contract;
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json;

    public static class DocumentDBHelpers
    {
        public static async Task DeleteDocument(this IDocumentDbRepository repository, string collection, string partitionKeyValue, params dynamic[] procedureParams)
        {
            var continuation = true;
            var totalDocumentDeleted = 0;

            Console.WriteLine("Query -> {0}", procedureParams);

            while (continuation)
            {
                var response = await repository.ExecuteStoredProcedureAsync<StorProcResponse>(collection, ApplicationConstant.BulkDeleteStoreProcName, partitionKeyValue, procedureParams);

                continuation = response.continuation;
                totalDocumentDeleted = totalDocumentDeleted + Convert.ToInt32(response.deleted);
            }

            Console.WriteLine("Document deletion Completed for Group Id - {0}, count - {1}", partitionKeyValue, totalDocumentDeleted);
        }

        public static async Task InsertDocument(this IDocumentDbRepository repository, string collection, string partitionKeyValue, IEnumerable<Document> documents)
        {
            var totalDocumentInserted = 0;

            while (documents.Count() > 0)
            {
                string argsJson = JsonConvert.SerializeObject(documents.ToArray());
                var args = new dynamic[] { JsonConvert.DeserializeObject<dynamic[]>(argsJson) };

                var response = await repository.ExecuteStoredProcedureAsync<int>(collection, ApplicationConstant.BulkImportStoreProcName, partitionKeyValue, args);

                var insertedDocument = Convert.ToInt32(response);
                documents = documents.Skip(insertedDocument);

                totalDocumentInserted = totalDocumentInserted + insertedDocument;
            }

            Console.WriteLine("Document insertion Completed for Group Id - {0}, count - {1}",partitionKeyValue, totalDocumentInserted);
        }
    }
}
