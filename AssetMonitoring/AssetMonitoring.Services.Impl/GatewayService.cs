namespace AssetMonitoring.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AssetMonitoring.Components.Repository;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.Enums;
    using AssetMonitoring.IoTHub.Services;
    using AssetMonitoring.Services.Impl.Mappings;

    public class GatewayService : IGatewayService
    {
        private readonly IRepository repository;
        private readonly IDeviceManagementService iotHubDeviceManagementService;
        private readonly ICloudToDeviceMessageService cloudToDeviceMessageService;
        private readonly IDocumentDbRepository documentDbRepository;
        private readonly ISensorRuleService sensorRuleService;

        public GatewayService(IRepository repository, IDeviceManagementService iotHubDeviceManagementService, ICloudToDeviceMessageService cloudToDeviceMessageService, IDocumentDbRepository documentDbRepository, ISensorRuleService sensorRuleService)
        {
            this.repository = repository;
            this.iotHubDeviceManagementService = iotHubDeviceManagementService;
            this.cloudToDeviceMessageService = cloudToDeviceMessageService;
            this.documentDbRepository = documentDbRepository;
            this.sensorRuleService = sensorRuleService;
        }

        async Task<OperationStatus> IGatewayService.Create(Gateway gateway)
        {
            var gateways = this.repository.Query<Entities.Gateway>();

            var isexistingGateway = gateways.Any(g => g.GatewayKey.Equals(gateway.GatewayKey, StringComparison.InvariantCultureIgnoreCase) || g.Name.Equals(gateway.Name, StringComparison.InvariantCultureIgnoreCase));

            if (isexistingGateway)
            {
                return new OperationStatus(StatusCode.Error, "Gateway already exists with given key or name.");
            }

            var gatwayEntity = new Entities.Gateway
            {
                Name = gateway.Name,
                Description = gateway.Description,
                GatewayKey = gateway.GatewayKey,
            };

            var iotHubDevice = await this.iotHubDeviceManagementService.AddDevice(new IotHubDevice { Id = gateway.GatewayKey });

            // update gateway meta-data.
            if (!string.IsNullOrWhiteSpace(gateway.Description))
            {
                var gatewayMetadata = new Dictionary<string, string>();
                gatewayMetadata.Add("Description", gateway.Description);
                await this.iotHubDeviceManagementService.UpdateDeviceMetadata(gateway.GatewayKey, gatewayMetadata);
            }

            gatwayEntity.IotHubAccessTocken = iotHubDevice.Authentication;
            this.repository.Persist(gatwayEntity);

            return new OperationStatus();
        }

        async Task<OperationStatus> IGatewayService.Delete(int gatewayId)
        {
            var gateway = this.repository.Read<Entities.Gateway>(gatewayId);

            if (gateway == null)
            {
                return new OperationStatus(StatusCode.Error, "Gateway does not exist for given id.");
            }

            await this.iotHubDeviceManagementService.DeleteDevice(gateway.GatewayKey);

            var gatewayRuleIds = this.repository.Query<Entities.SensorRule>().Where(s => s.MinThreshold.Equals(gateway.GatewayKey)).Select(s => s.Id);

            foreach (var gatewayRuleId in gatewayRuleIds)
            {
              await this.sensorRuleService.Delete(gatewayRuleId);
            }

            this.repository.Delete(gateway);

            return new OperationStatus();
        }

        Gateway IGatewayService.Get(int gatewayId)
        {
            var gateway = this.repository.Read<Entities.Gateway>(gatewayId);

            return new GatewayMapping().Map(gateway);
        }

        async Task<IotHubGateway> IGatewayService.GetIotHubGateway(string gatewayKey)
        {
            var gatewayEntity = this.repository.Query<Entities.Gateway>().FirstOrDefault(g => g.GatewayKey.Equals(gatewayKey));

            if (gatewayEntity != null)
            {
                // enable gateway.
                await this.iotHubDeviceManagementService.EnableDevice(gatewayKey);

                await this.SendGatewayMessage(gatewayKey);

                var gateway = new IotHubGatewayMapping().Map(gatewayEntity);
                gateway.DeviceConnectionString = string.Format("HostName={0};DeviceId={1};SharedAccessKey={2}", gateway.IotHubHostName, gatewayKey, gateway.IotHubAccessTocken);

                gateway.Capabilities = this.repository.Query<Entities.Capability>().Select(s => new Capability
                {
                    Id = s.Id,
                    Name = s.Name,
                    Unit = s.Unit
                }).ToList();

                return gateway;
            }

            return null;
        }

        List<Gateway> IGatewayService.GetAll()
        {
            var gateway = this.repository.Query<Entities.Gateway>();

            return new GatewayMapping().Map(gateway).ToList();
        }

        async Task<OperationStatus> IGatewayService.Update(Gateway gateway)
        {
            var gatewayEntity = this.repository.Read<Entities.Gateway>(gateway.Id);

            if (gatewayEntity == null)
            {
                return new OperationStatus(StatusCode.Error, "Gateway does not exist for given id.");
            }

            if (gatewayEntity.Description != gateway.Description && !string.IsNullOrWhiteSpace(gateway.Description))
            {
                // update gateway meta-data.
                var gatewayMetadata = new Dictionary<string, string>();
                gatewayMetadata.Add("Description", gateway.Description);
                await this.iotHubDeviceManagementService.UpdateDeviceMetadata(gateway.GatewayKey, gatewayMetadata);
            }

            var isexistingGateway = this.repository.Query<Entities.Gateway>().Any(g => g.Name.Equals(gateway.Name, StringComparison.InvariantCultureIgnoreCase) && g.Id != gateway.Id);

            if (isexistingGateway)
            {
                return new OperationStatus(StatusCode.Error, "Gateway already exists with given name.");
            }

            gatewayEntity.Name = gateway.Name;
            gatewayEntity.Description = gateway.Description;

            this.repository.Persist(gatewayEntity);

            return new OperationStatus();
        }

        async Task IGatewayService.GatewayMessage<T>(DeviceMessageStatus status, List<T> messageData, List<string> gatewayKeys, bool allowDisconnectedDevice)
        {
            if (gatewayKeys == null)
            {
                gatewayKeys = this.repository.Query<Entities.Gateway>().Select(g => g.GatewayKey).ToList();
            }

            foreach (var gatewayKey in gatewayKeys)
            {
                var messageProperty = new Dictionary<string, string>();
                messageProperty.Add("Status", status.ToString());

                var batcheMessageDatas = messageData
               .Select((x, i) => new { Index = i, Value = x })
               .GroupBy(x => x.Index / 100)
               .Select(x => x.Select(v => v.Value).ToList())
               .ToList();

                foreach (var batcheMessageData in batcheMessageDatas)
                {
                    await this.cloudToDeviceMessageService.SendDeviceMessage(gatewayKey, batcheMessageData, messageProperty, allowDisconnectedDevice);
                }
            }
        }

        async Task<List<Gateway>> IGatewayService.GetAllOnlineGateway()
        {
            var gatwayKeys = await this.iotHubDeviceManagementService.GetAllConnectedDevice();

            var gateway = this.repository.Query<Entities.Gateway>().Where(g => gatwayKeys.Any(key => key.Equals(g.GatewayKey)));

            return new GatewayMapping().Map(gateway).ToList();
        }

        private List<EnableSensor> GetAllEnabledSensors()
        {
            var sensor = this.repository.Query<Entities.Sensor>().Where(s => s.Asset != null).AsQueryable();
            return new EnableSensorMapping().Map(sensor).ToList();
        }

        private async Task SendGatewayMessage(string gatewayKey)
        {
            // sens attach sensors message
            var enabledSensors = this.GetAllEnabledSensors();
            if (enabledSensors.Count > 0)
            {
                await (this as IGatewayService).GatewayMessage(DeviceMessageStatus.AttachSensor, enabledSensors, new List<string> { gatewayKey }, true);
            }

            // send attach sensor type message.
            var enableSensorTypes = this.repository.Query<Entities.SensorType>()
                .Select(s => new EnableSensorType
                {
                    SensorType = s.Name,
                    SensorCapabilities = s.Capabilities.Select(c => new SensorCapability { Id = c.Id, Name = c.Name }).ToList()
                }).ToList();
            if (enableSensorTypes.Count > 0)
            {
                await (this as IGatewayService).GatewayMessage(DeviceMessageStatus.AttachSensorType, enableSensorTypes, new List<string> { gatewayKey }, true);
            }
        }
    }
}
