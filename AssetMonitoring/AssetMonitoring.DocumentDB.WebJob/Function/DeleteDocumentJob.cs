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
    using Microsoft.Azure.WebJobs;
    using Newtonsoft.Json;

    public class DeleteDocumentJob
    {
        private readonly IDocumentDbRepository repository;
        private readonly ApplicationInsightsLogger applicationInsightsLogger;

        public DeleteDocumentJob(IDocumentDbRepository repository, ApplicationInsightsLogger applicationInsightsLogger)
        {
            this.repository = repository;
            this.applicationInsightsLogger = applicationInsightsLogger;
        }

        public async Task DeleteAlertDocument([QueueTrigger("delete-alert-document")] DeleteRuleAlertDetail deleteRuleAlertDetail, TextWriter log)
        {
            try
            {
                var query = "SELECT c._self FROM c";

                if (deleteRuleAlertDetail.SensorRuleIds.Count > 0)
                {
                    var groupedFilterValues = deleteRuleAlertDetail.SensorRuleIds
                                        .Select((x, i) => new { Index = i, Value = x })
                                        .GroupBy(x => x.Index / 100)
                                        .Select(x => x.Select(v => v.Value.ToString()))
                                        .ToList();

                    foreach (var groupedFilterValue in groupedFilterValues)
                    {
                        var values = string.Join(",", groupedFilterValue);

                        var assetQuery = query + string.Format(" where c.sensorruleid in ({0}) ", values);

                        await this.repository.DeleteDocument(ApplicationConstant.DocumentDbAlertDataCollection, deleteRuleAlertDetail.GroupId.ToString(), assetQuery);
                    }
                }
                else
                {
                    await this.repository.DeleteDocument(ApplicationConstant.DocumentDbAlertDataCollection, deleteRuleAlertDetail.GroupId.ToString(), query);
                }
            }
            catch (Exception ex)
            {
                this.applicationInsightsLogger.LogException(ex, deleteRuleAlertDetail);

                throw;
            }
        }

        public async Task DeleteSensorDocument([QueueTrigger("delete-sensor-document")] DeleteDocumentDetail deleteDocumentDetail, TextWriter log)
        {
            try
            {
                var query = "SELECT c._self FROM c";

                if (deleteDocumentDetail.AssetBarcodes.Count > 0 || deleteDocumentDetail.SensorKeys.Count > 0)
                {
                    var format = "\"{0}\"";

                    var filterValues = deleteDocumentDetail.AssetBarcodes.Count > 0 ? deleteDocumentDetail.AssetBarcodes :
                        deleteDocumentDetail.SensorKeys;

                    var filter = deleteDocumentDetail.AssetBarcodes.Count > 0 ? "c.AssetBarcode" : "c.SensorKey";

                    var groupedFilterValues = filterValues
                                        .Select((x, i) => new { Index = i, Value = x })
                                        .GroupBy(x => x.Index / 100)
                                        .Select(x => x.Select(v => string.Format(format, v.Value)).ToList().ToList())
                                        .ToList();

                    foreach (var groupedFilterValue in groupedFilterValues)
                    {
                        var values = string.Join(",", groupedFilterValue);

                        var assetQuery = query + string.Format(" where {0} in ({1}) and c.Timestamp < {2} ", filter, values, JsonConvert.SerializeObject(DateTime.UtcNow));

                        await this.repository.DeleteDocument(ApplicationConstant.DocumentDbSensorDataCollection, deleteDocumentDetail.GroupId.ToString(), assetQuery);
                    }
                }
                else
                {
                    await this.repository.DeleteDocument(ApplicationConstant.DocumentDbSensorDataCollection, deleteDocumentDetail.GroupId.ToString(), query);
                }
            }
            catch (Exception ex)
            {
                this.applicationInsightsLogger.LogException(ex, deleteDocumentDetail);

                throw;
            }
        }

        public async Task DeleteGroupGpsDocument([QueueTrigger("delete-gps-document")] int groupid, TextWriter log)
        {
            try
            {
                var query = "SELECT c._self FROM c";

                await this.repository.DeleteDocument(ApplicationConstant.DocumentDbGroupGpsDataCollection, groupid.ToString(), query);
            }
            catch (Exception ex)
            {
                this.applicationInsightsLogger.LogException(ex, groupid);

                throw;
            }
        }
    }
}
