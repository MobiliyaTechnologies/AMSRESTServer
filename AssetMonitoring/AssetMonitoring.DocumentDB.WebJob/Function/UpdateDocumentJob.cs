namespace AssetMonitoring.DocumentDB.WebJob.Function
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using AssetMonitoring.Components.Repository;
    using AssetMonitoring.Contracts.DocumentDbContract;
    using AssetMonitoring.DocumentDB.WebJob.Logger;
    using AssetMonitoring.DocumentDB.WebJob.Utilities;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Linq;
    using Microsoft.Azure.WebJobs;
    using Newtonsoft.Json;

    public class UpdateDocumentJob
    {
        private readonly IDocumentDbRepository repository;
        private readonly ApplicationInsightsLogger applicationInsightsLogger;

        public UpdateDocumentJob(IDocumentDbRepository repository, ApplicationInsightsLogger applicationInsightsLogger)
        {
            this.repository = repository;
            this.applicationInsightsLogger = applicationInsightsLogger;
        }

        public async Task UpdateSensorDocument([QueueTrigger("updated-sensor-group-id")] UpdateDocumentDetail updateDocumentDetail, TextWriter log)
        {
            try
            {
                var query = "SELECT * FROM c";
                var currentTimestamp = DateTime.UtcNow;

                if (updateDocumentDetail.GroupAssets != null && updateDocumentDetail.GroupAssets.Count > 0)
                {
                    foreach (var groupAssets in updateDocumentDetail.GroupAssets)
                    {
                        if (groupAssets.Value.Count > 0)
                        {
                            var format = "\"{0}\"";

                            var groupedFilterValues = groupAssets.Value
                                                .Select((x, i) => new { Index = i, Value = x })
                                                .GroupBy(x => x.Index / 99)
                                                .Select(x => x.Select(v => string.Format(format, v.Value)).ToList())
                                                .ToList();

                            foreach (var groupedFilterValue in groupedFilterValues)
                            {
                                var assetValues = string.Join(",", groupedFilterValue);

                                var assetQuery = query + string.Format(" where c.AssetBarcode in ({0}) and c.Timestamp < {1} ", assetValues, JsonConvert.SerializeObject(currentTimestamp));

                                await this.UpdateDocument(assetQuery, groupAssets.Key.ToString(), updateDocumentDetail.NewGroupId.ToString());

                                await this.repository.DeleteDocument(ApplicationConstant.DocumentDbSensorDataCollection, groupAssets.Key.ToString(), assetQuery);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.applicationInsightsLogger.LogException(ex, updateDocumentDetail);

                throw;
            }
        }

        private async Task UpdateDocument(string query, string oldGroupId, string newGroupId)
        {
            var documentQuery = this.repository.GetDocumentByQuery<Document>(ApplicationConstant.DocumentDbSensorDataCollection, query, oldGroupId).AsDocumentQuery();

            while (documentQuery.HasMoreResults)
            {
                var documents = (await documentQuery.ExecuteNextAsync<Document>()).ToList();

                foreach (var doc in documents)
                {
                    doc.SetPropertyValue("GroupId", newGroupId);
                    doc.SetPropertyValue("id", null);
                }

                await this.repository.InsertDocument(ApplicationConstant.DocumentDbSensorDataCollection, newGroupId, documents);
            }
        }
    }
}
