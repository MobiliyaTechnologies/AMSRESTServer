namespace AssetMonitoring.Components.Repository.DocumentDB
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using AssetMonitoring.Utilities;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;

    public class DocumentDbRepository : IDocumentDbRepository
    {
        private readonly DocumentClient client;

        public DocumentDbRepository()
        {
            this.client = new DocumentClient(new Uri(ApplicationConfiguration.DocumentDbEndpoint), ApplicationConfiguration.DocumentDbAuthKey);
        }

        async Task IDocumentDbRepository.InitializeDocumentDB()
        {
            await this.CreateDatabaseIfNotExistsAsync(ApplicationConstant.DocumentDbDatabase);

            await this.CreateCollectionIfNotExistsAsync(ApplicationConstant.DocumentDbDatabase, ApplicationConstant.DocumentDbSensorDataCollection, "/GroupId");

            await this.CreateCollectionIfNotExistsAsync(ApplicationConstant.DocumentDbDatabase, ApplicationConstant.DocumentDbGroupGpsDataCollection, "/groupid");

            await this.CreateCollectionIfNotExistsAsync(ApplicationConstant.DocumentDbDatabase, ApplicationConstant.DocumentDbAlertDataCollection, "/groupid");

            await this.CreateBulkDeleteStorePorcedure();
            await this.CreateBulkImportStorePorcedure();
        }

        IQueryable<T> IDocumentDbRepository.Query<T>(string collection, string partitionKey, string continuationToken, int pageSize)
        {
            FeedOptions feedOptions = new FeedOptions { MaxItemCount = pageSize };

            if (!string.IsNullOrWhiteSpace(partitionKey))
            {
                feedOptions.PartitionKey = new PartitionKey(partitionKey);
            }
            else
            {
                feedOptions.EnableCrossPartitionQuery = true;
            }

            if (!string.IsNullOrWhiteSpace(continuationToken))
            {
                feedOptions.RequestContinuation = continuationToken;
            }

            IQueryable<T> query = this.client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(ApplicationConstant.DocumentDbDatabase, collection), feedOptions);

            return query;
        }

        IQueryable<T> IDocumentDbRepository.GetDocumentByQuery<T>(string collection, string query, string partitionKey, string continuationToken, int pageSize)
        {
            FeedOptions feedOptions = new FeedOptions { MaxItemCount = pageSize };

            if (!string.IsNullOrWhiteSpace(partitionKey))
            {
                feedOptions.PartitionKey = new PartitionKey(partitionKey);
            }
            else
            {
                feedOptions.EnableCrossPartitionQuery = true;
            }

            if (!string.IsNullOrWhiteSpace(continuationToken))
            {
                feedOptions.RequestContinuation = continuationToken;
            }

            IQueryable<T> documents = this.client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(ApplicationConstant.DocumentDbDatabase, collection), query, feedOptions);

            return documents;
        }

        async Task<T> IDocumentDbRepository.ExecuteStoredProcedureAsync<T>(string collectionName, string storeProcedureName, string partitionKey, params dynamic[] procedureParams)
        {
            var splink = UriFactory.CreateStoredProcedureUri(ApplicationConstant.DocumentDbDatabase, collectionName, storeProcedureName);

            var response = await this.client.ExecuteStoredProcedureAsync<string>(splink, new RequestOptions { PartitionKey = new PartitionKey(partitionKey) }, procedureParams);

            return JsonConvert.DeserializeObject<T>(response.Response);
        }

        private async Task CreateDatabaseIfNotExistsAsync(string databaseId)
        {
            try
            {
                await this.client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await this.client.CreateDatabaseAsync(new Database { Id = databaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync(string databaseId, string collectionId, string partitionKey = null)
        {
            try
            {
                await this.client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var documentCollection = new DocumentCollection { Id = collectionId };

                    if (!string.IsNullOrWhiteSpace(partitionKey))
                    {
                        documentCollection.PartitionKey.Paths.Add(partitionKey);
                    }

                    await this.client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseId),
                        documentCollection,
                        new RequestOptions { OfferThroughput = ApplicationConfiguration.DocumentDbOfferThroughput });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateBulkDeleteStorePorcedure()
        {
            var spBody = this.GetEmbeddedResource("AssetMonitoring.Components.Repository.DocumentDB.StoreProcedure.BulkDelete.js");

            var storeProcedure = new StoredProcedure()
            {
                Id = ApplicationConstant.BulkDeleteStoreProcName,
                Body = spBody
            };

            await this.CreateStoreProcedure(ApplicationConstant.DocumentDbAlertDataCollection, storeProcedure);
            await this.CreateStoreProcedure(ApplicationConstant.DocumentDbSensorDataCollection, storeProcedure);
            await this.CreateStoreProcedure(ApplicationConstant.DocumentDbGroupGpsDataCollection, storeProcedure);
        }

        private async Task CreateBulkImportStorePorcedure()
        {
            var spBody = this.GetEmbeddedResource("AssetMonitoring.Components.Repository.DocumentDB.StoreProcedure.BulkImport.js");

            var storeProcedure = new StoredProcedure()
            {
                Id = ApplicationConstant.BulkImportStoreProcName,
                Body = spBody
            };

            await this.CreateStoreProcedure(ApplicationConstant.DocumentDbSensorDataCollection, storeProcedure);
        }

        private async Task CreateStoreProcedure(string collection, StoredProcedure storeProcedure)
        {
            // delete and recreate store procedure as update not supported.
            try
            {
                var splink = UriFactory.CreateStoredProcedureUri(ApplicationConstant.DocumentDbDatabase, collection, storeProcedure.Id);

               await this.client.DeleteStoredProcedureAsync(splink);
            }
            catch (Exception)
            {
                // ignore exception, throws if store procedure not exists.
            }

            var collectionURI = UriFactory.CreateDocumentCollectionUri(ApplicationConstant.DocumentDbDatabase, collection);

           await this.client.CreateStoredProcedureAsync(collectionURI, storeProcedure);
        }

        private string GetEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
