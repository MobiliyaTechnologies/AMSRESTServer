namespace AssetMonitoring.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AssetMonitoring.Components.Repository;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.Enums;
    using AssetMonitoring.Services.Impl.Mappings;
    using AssetMonitoring.Utilities;

    public sealed class SensorTypeService : ISensorTypeService
    {
        private readonly IRepository repository;
        private readonly IGatewayService gatewayService;

        public SensorTypeService(IRepository repository, IGatewayService gatewayService)
        {
            this.repository = repository;
            this.gatewayService = gatewayService;
        }

        async Task<OperationStatus> ISensorTypeService.Create(SensorType sensorType)
        {
            var isexistingSensorType = this.repository.Query<Entities.SensorType>().Any(g => g.Name.Equals(sensorType.Name, StringComparison.InvariantCultureIgnoreCase));

            if (isexistingSensorType)
            {
                return new OperationStatus(StatusCode.Error, "Sensor type already exists with given name.");
            }

            var sensorTypeEntity = new Entities.SensorType()
            {
                Name = sensorType.Name,
                Description = sensorType.Description,
            };

            foreach (var capabilityId in sensorType.CapabilityIds)
            {
                var capabilityEntity = this.repository.Read<Entities.Capability>(capabilityId);

                if (capabilityEntity == null)
                {
                    return new OperationStatus(StatusCode.Error, string.Format("Sensor capability does not exist for id - {0}", capabilityId));
                }

                sensorTypeEntity.Capabilities.Add(capabilityEntity);
            }

            // add gateway capability.
            var gatewayCapability = this.repository.Query<Entities.Capability>().First(c => c.Name.Equals(ApplicationConstant.GatewayCapability));

            if (!sensorType.CapabilityIds.Any(id => id == gatewayCapability.Id))
            {
                sensorTypeEntity.Capabilities.Add(gatewayCapability);
            }

            var enableSensorType = new List<EnableSensorType> { new EnableSensorType
            {
                SensorType = sensorTypeEntity.Name,
                SensorCapabilities = sensorTypeEntity.Capabilities.Select(c => new SensorCapability { Id = c.Id, Name = c.Name }).ToList()
            } };

            await this.gatewayService.GatewayMessage(DeviceMessageStatus.AttachSensorType, enableSensorType);

            this.repository.Persist<Entities.SensorType>(sensorTypeEntity);
            return new OperationStatus();
        }

        async Task<OperationStatus> ISensorTypeService.Delete(int sensorTypeId)
        {
            var sensorTypeEntity = this.repository.Read<Entities.SensorType>(sensorTypeId);

            if (sensorTypeEntity == null)
            {
                return new OperationStatus(StatusCode.Error, "Sensor type does not exist for given sensor type identifier.");
            }

            var detachSensor = sensorTypeEntity.Sensors.Select(sensor => new EnableSensor { SensorKey = sensor.SensorKey }).ToList();

            await this.gatewayService.GatewayMessage(DeviceMessageStatus.DetachSensor, detachSensor);

            var detachSensorType = new List<EnableSensorType> { new EnableSensorType { SensorType = sensorTypeEntity.Name } };

            await this.gatewayService.GatewayMessage(DeviceMessageStatus.DetachSensorType, detachSensorType);

            this.repository.Delete(sensorTypeEntity);

            return new OperationStatus(StatusCode.Ok, "Sensor type deleted successfully.");
        }

        SensorType ISensorTypeService.Get(int sensorTypeId)
        {
            var sensorType = this.repository.Read<Entities.SensorType>(sensorTypeId);

            return new SensorTypeMapping().Map(sensorType);
        }

        List<SensorType> ISensorTypeService.GetAll()
        {
            var sensorType = this.repository.Query<Entities.SensorType>();

            return new SensorTypeMapping().Map(sensorType).ToList();
        }

        OperationStatus ISensorTypeService.Update(SensorType sensorType)
        {
            var sensorTypeEntity = this.repository.Read<Entities.SensorType>(sensorType.Id);

            if (sensorTypeEntity == null)
            {
                return new OperationStatus(StatusCode.Error, "Sensor type does not exist for given sensor type identifier.");
            }

            sensorTypeEntity.Name = sensorType.Name;
            sensorTypeEntity.Description = sensorType.Description;

            this.repository.Persist(sensorTypeEntity);
            return new OperationStatus(StatusCode.Ok, "Sensor type update successfully.");
        }

        async Task<OperationStatus> ISensorTypeService.SetCapabilitiesToSensorType(int sensorTypeId, List<int> capabilityIds)
        {
            var sensorTypeEntity = this.repository.Read<Entities.SensorType>(sensorTypeId);

            if (sensorTypeEntity == null)
            {
                return new OperationStatus(StatusCode.Error, "Sensor type does not exist for given sensor type identifier.");
            }

            var deletedCapabilities = sensorTypeEntity.Capabilities.Where(c => !capabilityIds.Any(id => id == c.Id)).ToList();

            foreach (var deletedCapability in deletedCapabilities.ToList())
            {
                sensorTypeEntity.Capabilities.Remove(deletedCapability);
            }

            var newCapabilityIds = capabilityIds.Where(id => !sensorTypeEntity.Capabilities.Any(sc => sc.Id == id));

            foreach (var newCapabilityId in newCapabilityIds)
            {
                var capabilityEntity = this.repository.Read<Entities.Capability>(newCapabilityId);

                if (capabilityEntity == null)
                {
                    return new OperationStatus(StatusCode.Error, string.Format("Sensor capability does not exist for id - {0}", newCapabilityId));
                }

                sensorTypeEntity.Capabilities.Add(capabilityEntity);
            }

            var enableSensorType = new List<EnableSensorType> {new EnableSensorType
            {
                SensorType = sensorTypeEntity.Name,
                SensorCapabilities = sensorTypeEntity.Capabilities.Select(c => new SensorCapability { Id = c.Id, Name = c.Name }).ToList()
            }
        };

            await this.gatewayService.GatewayMessage(DeviceMessageStatus.AttachSensorType, enableSensorType);

            this.repository.Persist(sensorTypeEntity);
            return new OperationStatus();
        }
    }
}
