namespace AssetMonitoring.Services.Impl
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AssetMonitoring.Components.Repository;
    using AssetMonitoring.Components.Repository.DocumentDB;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.DocumentDbContract;
    using AssetMonitoring.Contracts.Enums;
    using AssetMonitoring.Services.Impl.Mappings;
    using AssetMonitoring.Utilities;

    public class SensorGroupService : ISensorGroupService
    {
        private readonly IRepository repository;
        private readonly IGatewayService gatewayService;
        private readonly IGroupAlertService groupAlertService;
        private readonly ISensorRuleService sensorRuleService;
        private readonly IDocumentDbRepository documentDbRepository;
        private readonly IQueueStorageService queueStorageService;
        private readonly IAlertService alertService;

        public SensorGroupService(IRepository repository, IGatewayService gatewayService, IGroupAlertService groupAlertService, ISensorRuleService sensorRuleService, IDocumentDbRepository documentDbRepository, IQueueStorageService queueStorageService, IAlertService alertService)
        {
            this.repository = repository;
            this.gatewayService = gatewayService;
            this.groupAlertService = groupAlertService;
            this.sensorRuleService = sensorRuleService;
            this.documentDbRepository = documentDbRepository;
            this.queueStorageService = queueStorageService;
            this.alertService = alertService;
        }

        async Task<OperationStatus> ISensorGroupService.AddAsset(int groupId, List<int> assetIds)
        {
            var group = this.repository.Read<Entities.SensorGroup>(groupId);

            if (group == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor group does not exist for id - {0}.", groupId));
            }

            var enableSensors = new List<EnableSensor>();
            var enableSensorMapping = new EnableSensorMapping();
            var updateDocumentDetail = new UpdateDocumentDetail() { NewGroupId = groupId };

            foreach (var assetId in assetIds)
            {
                var asset = this.repository.Read<Entities.Asset>(assetId);

                if (asset == null)
                {
                    return new OperationStatus(StatusCode.Error, string.Format("Asset does not exist for id - {0}.", assetId));
                }

                var oldGroupId = asset.SensorGroupId ?? default(int);

                if (oldGroupId != groupId)
                {
                    if (!updateDocumentDetail.GroupAssets.ContainsKey(oldGroupId))
                    {
                        updateDocumentDetail.GroupAssets.Add(oldGroupId, new List<string>());
                    }

                    updateDocumentDetail.GroupAssets[oldGroupId].Add(asset.AssetBarcode);
                }

                asset.SensorGroupId = groupId;
                this.repository.Persist(asset);

                foreach (var sensor in asset.Sensors.ToList())
                {
                    sensor.SensorGroupId = groupId;
                    this.repository.Persist(sensor);
                }

                enableSensors.AddRange(enableSensorMapping.Map(asset.Sensors.AsQueryable()));
            }

            if (enableSensors.Count > 0)
            {
                await this.gatewayService.GatewayMessage(DeviceMessageStatus.AttachSensor, enableSensors);
            }

            // update document db sensor data.
            await this.queueStorageService.SendMessage(ApplicationConstant.UpdateDocumentGroupIdQueueName, updateDocumentDetail);

            return new OperationStatus();
        }

        async Task<OperationStatus> ISensorGroupService.RemoveAsset(int groupId, List<int> assetIds)
        {
            var group = this.repository.Read<Entities.SensorGroup>(groupId);

            if (group == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor group does not exist for id - {0}.", groupId));
            }

            var enableSensors = new List<EnableSensor>();
            var enableSensorMapping = new EnableSensorMapping();
            var assetBarcodes = new List<string>();

            foreach (var assetId in assetIds)
            {
                var asset = this.repository.Read<Entities.Asset>(assetId);

                if (asset == null)
                {
                    return new OperationStatus(StatusCode.Error, string.Format("Asset does not exist for id - {0}.", assetId));
                }

                asset.SensorGroupId = null;
                this.repository.Persist(asset);

                foreach (var sensor in asset.Sensors.ToList())
                {
                    sensor.SensorGroupId = null;
                    this.repository.Persist(sensor);
                }

                enableSensors.AddRange(enableSensorMapping.Map(asset.Sensors.AsQueryable()));
                assetBarcodes.Add(asset.AssetBarcode);
            }

            this.repository.Flush();

            var response = await this.sensorRuleService.ResetRules(groupId);

            if (response.StatusCode != StatusCode.Ok)
            {
                return response;
            }

            if (enableSensors.Count > 0)
            {
                await this.gatewayService.GatewayMessage(DeviceMessageStatus.AttachSensor, enableSensors);
            }

            // delete document db sensor data.
            await this.DeleteDocumentDBSensorData(groupId, assetBarcodes);

            return new OperationStatus();
        }

        async Task<OperationStatus> ISensorGroupService.Create(SensorGroup sensorGroup)
        {
            var group = new Entities.SensorGroup
            {
                Name = sensorGroup.Name,
                Description = sensorGroup.Description
            };

            this.repository.Persist(group);
            this.repository.Flush();

            if (sensorGroup.AssetIds.Count > 0)
            {
                await (this as ISensorGroupService).AddAsset(group.Id, sensorGroup.AssetIds);
            }

            return new OperationStatus();
        }

        async Task<OperationStatus> ISensorGroupService.Delete(int sensorGroupId)
        {
            var group = this.repository.Read<Entities.SensorGroup>(sensorGroupId);

            if (group == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor group does not exist for id - {0}.", sensorGroupId));
            }

            var enableSensors = group.Sensors.Select(s => new EnableSensor() { SensorKey = s.SensorKey }).ToList();

            if (enableSensors.Count > 0)
            {
                await this.gatewayService.GatewayMessage(DeviceMessageStatus.DetachSensor, enableSensors);
            }

            // delete document db sensor data.
            await this.DeleteDocumentDBSensorData(sensorGroupId);

            // delete document db rule alert data.
            await this.alertService.DeleteAlerts(sensorGroupId);

            this.repository.Delete(group);

            await this.groupAlertService.DeleteGroupFilter(sensorGroupId);

            // delete document db group gps data.
            await this.queueStorageService.SendMessage<int>(ApplicationConstant.DeleteGroupGpsQueueName, sensorGroupId);

            return new OperationStatus();
        }

        SensorGroup ISensorGroupService.Get(int sensorGroupId)
        {
            var group = this.repository.Read<Entities.SensorGroup>(sensorGroupId);

            var sensorGroup = new SensorGroupMapping().Map(group);

            if (sensorGroup != null && sensorGroup.Sensors.Count > 0)
            {
                sensorGroup.CapabilityNames = group.Sensors.SelectMany(s => s.SensorType.Capabilities).Select(c => c.Name).Distinct().ToList();
            }

            return sensorGroup;
        }

        List<SensorGroup> ISensorGroupService.GetAll()
        {
            var groups = this.repository.Query<Entities.SensorGroup>();
            var sensorGroups = groups.Select(s => new SensorGroup
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                RuleCreationInProgress = s.RuleCreationInProgress
            }).ToList();

            return sensorGroups;
        }

        OperationStatus ISensorGroupService.Update(SensorGroup sensorGroup)
        {
            var group = this.repository.Read<Entities.SensorGroup>(sensorGroup.Id);

            if (group == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor group does not exist for id - {0}.", sensorGroup.Id));
            }

            group.Name = sensorGroup.Name;
            group.Description = sensorGroup.Description;

            this.repository.Persist(group);

            return new OperationStatus();
        }

        async Task<OperationStatus> ISensorGroupService.DetachSensors(int groupId)
        {
            var group = this.repository.Read<Entities.SensorGroup>(groupId);

            if (group == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor group does not exist for id - {0}.", groupId));
            }

            foreach (var sensor in group.Sensors.ToList())
            {
                sensor.SensorGroupId = null;
                sensor.AssetId = null;

                this.repository.Persist(sensor);
            }

            foreach (var sensorRule in group.SensorRules.ToList())
            {
                this.repository.Delete(sensorRule);
            }

            var detachSensor = group.Sensors.Select(s => new EnableSensor() { SensorKey = s.SensorKey }).ToList();

            if (detachSensor.Count > 0)
            {
                await this.gatewayService.GatewayMessage(DeviceMessageStatus.DetachSensor, detachSensor);
            }

            // delete document db sensor data.
            await this.DeleteDocumentDBSensorData(groupId);

            // delete document db rule alert data.
            await this.alertService.DeleteAlerts(groupId);

            group.RuleCreationInProgress = true;
            this.repository.Persist(group);

            await this.groupAlertService.ApplyGroupFilter(group.Id);

            return new OperationStatus();
        }

        async Task<List<GroupGpsDetail>> ISensorGroupService.GetAllGroupStartEndLocation()
        {
            var groupGpsDetails = this.repository.Query<Entities.SensorGroup>().Select(g => new GroupGpsDetail { GroupId = g.Id, GroupName = g.Name }).ToList();

            foreach (var groupGpsDetail in groupGpsDetails)
            {
                var start = (await this.documentDbRepository.Query<GroupLocation>(ApplicationConstant.DocumentDbGroupGpsDataCollection, groupGpsDetail.GroupId.ToString(), null, 1).OrderBy(g => g._ts).PaginateDocument()).Result.FirstOrDefault();

                if (start != null)
                {
                    groupGpsDetail.Gps.Add(new GpsDetail { Latitude = start.Latitude, Longitude = start.Longitude, Timestamp = start.Timestamp });

                    var end = (await this.documentDbRepository.Query<GroupLocation>(ApplicationConstant.DocumentDbGroupGpsDataCollection, groupGpsDetail.GroupId.ToString(), null, 1).OrderByDescending(g => g._ts).PaginateDocument()).Result.FirstOrDefault();

                    if (end != null)
                    {
                        groupGpsDetail.Gps.Add(new GpsDetail { Latitude = end.Latitude, Longitude = end.Longitude, Timestamp = end.Timestamp });
                    }
                }
            }

            return groupGpsDetails;
        }

        private async Task DeleteDocumentDBSensorData(int groupId, List<string> assetBarcode = null)
        {
            // delete documentDB sensor data
            var deleteDocumentDetail = new DeleteDocumentDetail
            {
                GroupId = groupId,
            };

            if (assetBarcode != null)
            {
                deleteDocumentDetail.AssetBarcodes = assetBarcode;
            }

            await this.queueStorageService.SendMessage(ApplicationConstant.DeleteDocumentQueueName, deleteDocumentDetail);
        }
    }
}
