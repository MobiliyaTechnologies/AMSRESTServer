namespace AssetMonitoring.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.DocumentDbContract;
    using AssetMonitoring.Utilities;
    using Components.Repository;
    using Mappings;
    using AssetMonitoring.Components.Repository.DocumentDB;

    public class AlertService : IAlertService
    {
        private readonly IRepository repository;
        private readonly IDocumentDbRepository documentDbRepository;
        private readonly IQueueStorageService queueStorageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertService" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="documentDbRepository">The document database repository.</param>
        /// <param name="queueStorageService">The queue storage service.</param>
        public AlertService(IRepository repository, IDocumentDbRepository documentDbRepository, IQueueStorageService queueStorageService)
        {
            this.repository = repository;
            this.queueStorageService = queueStorageService;
            this.documentDbRepository = documentDbRepository;
        }

        async Task IAlertService.DeleteAlerts(int groupId, List<int> sensorRuleIds)
        {
            var deleteRuleAlertDetail = new DeleteRuleAlertDetail();
            deleteRuleAlertDetail.GroupId = groupId;

            if (sensorRuleIds != null && sensorRuleIds.Count > 0)
            {
                deleteRuleAlertDetail.SensorRuleIds.AddRange(sensorRuleIds);
            }

            await this.queueStorageService.SendMessage(ApplicationConstant.DeleteRuleAlertQueueName, deleteRuleAlertDetail);
        }

        List<AlertDocument> IAlertService.GetAlertByGroup(int groupId)
        {
            var sensorRuleCapabilities = this.repository.Query<Entities.SensorRule>().Where(s => s.SensorGroupId == groupId).ToDictionary(s => s.Id, s => new { CapabilityFilterName = s.CapabilityFilter.Name, CapabilityName = s.CapabilityFilter.Capability.Name });

            var alerts = new List<AlertDocument>();

            if (sensorRuleCapabilities.Count == 0)
            {
                return alerts;
            }

            alerts = this.documentDbRepository.Query<AlertDocument>(ApplicationConstant.DocumentDbAlertDataCollection, groupId.ToString()).Where(a => sensorRuleCapabilities.Keys.Contains(a.sensorruleid)).OrderByDescending(a => a._ts).ToList();

            foreach (var alert in alerts)
            {
                if (sensorRuleCapabilities.ContainsKey(alert.sensorruleid))
                {
                    var sensorRuleCapability = sensorRuleCapabilities[alert.sensorruleid];

                    alert.capability = sensorRuleCapability.CapabilityName.Equals(sensorRuleCapability.CapabilityFilterName) ? sensorRuleCapability.CapabilityName : string.Format("{0}_{1}", sensorRuleCapability.CapabilityName, sensorRuleCapability.CapabilityFilterName);
                }
            }

            return alerts;
        }

        List<AlertDocument> IAlertService.GetAlerts(int groupId, string assetBarcode)
        {
            var alerts = this.documentDbRepository.Query<AlertDocument>(ApplicationConstant.DocumentDbAlertDataCollection, groupId.ToString()).Where(a => a.assetbarcode == assetBarcode).OrderByDescending(a => a._ts).ToList();

            return alerts;
        }

        async Task<AlertPaginationResult> IAlertService.GetPaginateAlert(AlertPaginationFilter alertPaginationFilter)
        {
            var pageSize = -1;

            var sensorRules = this.repository.Query<Entities.SensorRule>().Where(s => s.SensorGroupId == alertPaginationFilter.GroupId);

            if (alertPaginationFilter.PageSize.HasValue)
            {
                pageSize = alertPaginationFilter.PageSize.Value;
            }

            if (alertPaginationFilter.RuleId.HasValue)
            {
                sensorRules = sensorRules.Where(s => s.Id == alertPaginationFilter.RuleId.Value);
            }

            var sensorRuleCapabilities = sensorRules.ToDictionary(s => s.Id, s => new { CapabilityFilterName = s.CapabilityFilter.Name, CapabilityName = s.CapabilityFilter.Capability.Name });

            if (sensorRuleCapabilities.Count == 0)
            {
                return new AlertPaginationResult();
            }

            var alertQuery = this.documentDbRepository.Query<AlertDocument>(ApplicationConstant.DocumentDbAlertDataCollection, alertPaginationFilter.GroupId.ToString(), alertPaginationFilter.ResponseContinuation, pageSize).Where(a => sensorRuleCapabilities.Keys.Contains(a.sensorruleid));

            if (!string.IsNullOrWhiteSpace(alertPaginationFilter.AssetBarcode))
            {
                alertQuery = alertQuery.Where(a => a.assetbarcode == alertPaginationFilter.AssetBarcode);
            }

            var paginateAlert = await alertQuery.OrderByDescending(a => a._ts).PaginateDocument();

            var alertPaginationResult = new AlertPaginationResult
            {
                ResponseContinuation = paginateAlert.ResponseContinuation,
                Count = paginateAlert.Result.Count()
            };

            foreach (var pagedAlert in paginateAlert.Result)
            {
                if (sensorRuleCapabilities.ContainsKey(pagedAlert.sensorruleid))
                {
                    var sensorRuleCapability = sensorRuleCapabilities[pagedAlert.sensorruleid];

                    var alert = new Alert
                    {
                        Value = pagedAlert.value,
                        AssetBarcode = pagedAlert.assetbarcode,
                        Timestamp = pagedAlert.timestamp
                    };

                    alert.Capability = sensorRuleCapability.CapabilityName.Equals(sensorRuleCapability.CapabilityFilterName) ? sensorRuleCapability.CapabilityName : string.Format("{0}_{1}", sensorRuleCapability.CapabilityName, sensorRuleCapability.CapabilityFilterName);

                    alertPaginationResult.Result.Add(alert);
                }
            }

            return alertPaginationResult;
        }

        List<string> IAlertService.GetDamageAssetBarcodes(int? groupId)
        {
            string partitionKey = null;

            if (groupId.HasValue && groupId.Value > 0)
            {
                partitionKey = groupId.Value.ToString();
            }

            var damageAssetBarcodes = this.documentDbRepository.Query<AlertDocument>(ApplicationConstant.DocumentDbAlertDataCollection, partitionKey).Select(a => a.assetbarcode).ToList();

            return damageAssetBarcodes.Distinct().ToList();
        }

        List<GroupAlertPaginateFilter> IAlertService.GetGroupAlertFilter()
        {
            var groupSensorRules = this.repository.Query<Entities.SensorRule>().GroupBy(r => r.SensorGroupId).ToDictionary(s => s.Key, s => s.Select(r => new { RuleId = r.Id, CapabilityFilterName = r.CapabilityFilter.Name, CapabilityName = r.CapabilityFilter.Capability.Name }));

            var groupAlertFilters = (from g in this.repository.Query<Entities.SensorGroup>()
                                     select new GroupAlertPaginateFilter
                                     {
                                         GroupId = g.Id,
                                         GroupName = g.Name,
                                         AssetBarcodes = g.Assets.Select(a => a.AssetBarcode)
                                     }).ToList();

            foreach (var groupAlertFilter in groupAlertFilters)
            {
                if (groupSensorRules.ContainsKey(groupAlertFilter.GroupId))
                {
                    var groupRules = groupSensorRules[groupAlertFilter.GroupId];

                    groupAlertFilter.GroupRules = groupRules.Select(r => new GroupRuleAlertFilter { RuleId = r.RuleId, CapabilityName = r.CapabilityName.Equals(r.CapabilityFilterName) ? r.CapabilityName : string.Format("{0}_{1}", r.CapabilityName, r.CapabilityFilterName) }).ToList();
                }
            }

            return groupAlertFilters;
        }
    }
}
