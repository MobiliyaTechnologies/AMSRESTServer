namespace AssetMonitoring.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AssetMonitoring.Components.Repository;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.DocumentDbContract;
    using AssetMonitoring.Contracts.Enums;
    using AssetMonitoring.Services.Impl.Mappings;
    using AssetMonitoring.Utilities;

    public sealed class AssetService : IAssetService
    {
        private readonly IRepository repository;
        private readonly IGatewayService gatewayService;
        private readonly ISensorRuleService sensorRuleService;
        private readonly IQueueStorageService queueStorageService;
        private readonly IAlertService alertService;

        public AssetService(IRepository repository, IGatewayService gatewayService, ISensorRuleService sensorRuleService, IQueueStorageService queueStorageService, IAlertService alertService)
        {
            this.repository = repository;
            this.gatewayService = gatewayService;
            this.sensorRuleService = sensorRuleService;
            this.queueStorageService = queueStorageService;
            this.alertService = alertService;
        }

        async Task<OperationStatus> IAssetService.Create(Asset asset)
        {
            var sensor = this.repository.Query<Entities.Sensor>().FirstOrDefault(s => s.SensorKey.Equals(asset.SensorKey));

            if (sensor == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor does not exist for key - {0}.", asset.SensorKey));
            }

            if (sensor.AssetId.HasValue)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor {0} already associated with asset {1}, it can be associate with only one asset at a time.", sensor.SensorKey, sensor.Asset.AssetBarcode));
            }

            var assetEntity = this.repository.Query<Entities.Asset>().FirstOrDefault(a => a.AssetBarcode.Equals(asset.AssetBarcode));

            if (assetEntity == null)
            {
                assetEntity = new Entities.Asset
                {
                    AssetBarcode = asset.AssetBarcode,
                };

                assetEntity.Sensors.Add(sensor);
            }
            else if (assetEntity.Sensors.Any(s => s.Id == sensor.Id))
            {
                return new OperationStatus();
            }
            else
            {
                sensor.SensorGroupId = assetEntity.SensorGroupId;
                assetEntity.Sensors.Add(sensor);
            }

            sensor.Asset = assetEntity;

            var enableSensor = new EnableSensorMapping().Map(sensor);
            await this.gatewayService.GatewayMessage(DeviceMessageStatus.AttachSensor, new List<EnableSensor> { enableSensor });

            this.repository.Persist(assetEntity);
            return new OperationStatus();
        }

        async Task<OperationStatus> IAssetService.Delete(string assetBarcode)
        {
            var asset = this.repository.Query<Entities.Asset>().FirstOrDefault(a => a.AssetBarcode.Equals(assetBarcode));

            if (asset == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Asset does not exist for barcode - {0}.", assetBarcode));
            }

            if (asset.Sensors.Any())
            {
                var detachSensor = asset.Sensors.Select(s => new EnableSensor { SensorKey = s.SensorKey }).ToList();
                await this.gatewayService.GatewayMessage(DeviceMessageStatus.DetachSensor, detachSensor);
            }

            this.repository.Delete(asset);

            return new OperationStatus();
        }

        async Task<OperationStatus> IAssetService.DetachSensor(Asset sensorAsset)
        {
            var sensor = this.repository.Query<Entities.Sensor>().FirstOrDefault(s => s.SensorKey.Equals(sensorAsset.SensorKey));

            if (sensor == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor does not exist for key - {0}.", sensorAsset.SensorKey));
            }

            var asset = this.repository.Query<Entities.Asset>().FirstOrDefault(a => a.AssetBarcode.Equals(sensorAsset.AssetBarcode));

            if (asset == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Asset does not exist for barcode - {0}.", sensorAsset.AssetBarcode));
            }

            var sensorGroupId = sensor.SensorGroupId;

            sensor.SensorGroupId = null;
            sensor.AssetId = null;
            this.repository.Persist(sensor);

            if (asset.SensorGroupId.HasValue)
            {
                this.repository.Flush();

                var response = await this.sensorRuleService.ResetRules(asset.SensorGroupId.Value);

                if (response.StatusCode != StatusCode.Ok)
                {
                    return response;
                }
            }

            var enableSensor = new EnableSensor { SensorKey = sensorAsset.SensorKey };
            await this.gatewayService.GatewayMessage(DeviceMessageStatus.DetachSensor, new List<EnableSensor> { enableSensor });

            if (sensorGroupId.HasValue)
            {
                // delete documentDB sensor data
                var deleteDocumentDetail = new DeleteDocumentDetail
                {
                    GroupId = sensorGroupId.Value,
                    SensorKeys = new List<string> { sensor.SensorKey }
                };

                await this.queueStorageService.SendMessage(ApplicationConstant.DeleteDocumentQueueName, deleteDocumentDetail);
            }

            return new OperationStatus();
        }

        GroupAsset IAssetService.Get(string assetBarcode)
        {
            var asset = this.repository.Query<Entities.Asset>().FirstOrDefault(a => a.AssetBarcode.Equals(assetBarcode));
            return new GroupAssetMapping().Map(asset);
        }

        List<GroupAsset> IAssetService.GetAll()
        {
            var assets = this.repository.Query<Entities.Asset>();
            return new GroupAssetMapping().Map(assets).ToList();
        }

        List<GroupAsset> IAssetService.GetAllDamagAsset(int? groupId)
        {
            var assetBarcodes = this.alertService.GetDamageAssetBarcodes(groupId);
            var assets = this.repository.Query<Entities.Asset>().Where(a => assetBarcodes.Any(ab => ab.Equals(a.AssetBarcode)));

            return new GroupAssetMapping().Map(assets).ToList();
        }

        List<AssetStatus> IAssetService.GetAssetStatus(string assetBarcode, OperationStatus operationStatus)
        {
            var asset = this.repository.Query<Entities.Asset>().FirstOrDefault(a => a.AssetBarcode.Equals(assetBarcode));
            var assetStatus = new List<AssetStatus>();

            string errorMessage = null;

            if (asset == null)
            {
                errorMessage = string.Format("Asset {0} does not exist.", assetBarcode);
            }
            else if (!asset.SensorGroupId.HasValue)
            {
                errorMessage = string.Format("Asset {0} is not associated with any group.", assetBarcode);
            }
            else if (!asset.Sensors.Any())
            {
                errorMessage = string.Format("Sensors not attached with asset {0}.", assetBarcode);
            }

            if (errorMessage != null)
            {
                operationStatus.StatusCode = StatusCode.Error;
                operationStatus.Message = errorMessage;
                return assetStatus;
            }

            var assetSensorKeys = asset.Sensors.Select(s => s.SensorKey).ToList();

            var alerts = this.alertService.GetAlerts(asset.SensorGroupId.Value, assetBarcode).GroupBy(a => a.sensorruleid).Select(g => g.FirstOrDefault());

            var alertRuleIds = alerts.Where(a => a != null).Select(a => a.sensorruleid);

            var sensorRules = this.repository.Query<Entities.SensorRule>().Where(s => alertRuleIds.Any(id => id == s.Id)).ToDictionary(s => s.Id.ToString(), s => new { MinThreshold = s.MinThreshold, MaxThreshold = s.MaxThreshold, CapabilityFilterName = s.CapabilityFilter.Name, CapabilityName = s.CapabilityFilter.Capability.Name });

            foreach (var alert in alerts)
            {
                if (alert == null || !sensorRules.ContainsKey(alert.sensorruleid.ToString()))
                {
                    continue;
                }

                var status = new AssetStatus();

                var sensorRule = sensorRules[alert.sensorruleid.ToString()];

                status.Capability = sensorRule.CapabilityName;
                status.Timestamp = alert.timestamp;
                status.MinThreashold = sensorRule.MinThreshold;
                status.MaxThreashold = sensorRule.MaxThreshold;
                status.Value = alert.value;
                status.CapabilityFilter = sensorRule.CapabilityFilterName;

                assetStatus.Add(status);
            }

            errorMessage = string.Empty;
            return assetStatus;
        }
    }
}
