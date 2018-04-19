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

    public sealed class SensorService : ISensorService
    {
        private readonly IRepository repository;
        private readonly IGatewayService gatewayService;

        public SensorService(IRepository repository, IGatewayService gatewayService)
        {
            this.repository = repository;
            this.gatewayService = gatewayService;
        }

        OperationStatus ISensorService.Create(Sensor sensor)
        {
            var sensorType = this.repository.Read<Entities.SensorType>(sensor.SensorTypeId);

            if (sensorType == null)
            {
                return new OperationStatus(Contracts.Enums.StatusCode.Error, string.Format("Sensor type does not exist for id - {0}.", sensor.SensorTypeId));
            }

            var isexistingSensor = this.repository.Query<Entities.Sensor>().Any(g => g.SensorKey.Equals(sensor.SensorKey, StringComparison.InvariantCultureIgnoreCase) || g.Name.Equals(sensor.Name, StringComparison.InvariantCultureIgnoreCase));

            if (isexistingSensor)
            {
                return new OperationStatus(StatusCode.Error, "Sensor already exists with given key or name.");
            }

            var sensorEntity = new Entities.Sensor
            {
                Name = sensor.Name,
                Description = sensor.Description,
                SensorKey = sensor.SensorKey,
                SensorType = sensorType
            };

            this.repository.Persist(sensorEntity);
            return new OperationStatus();
        }

        async Task<OperationStatus> ISensorService.Delete(int sensorId)
        {
            var sensor = this.repository.Read<Entities.Sensor>(sensorId);

            if (sensor == null)
            {
                return new OperationStatus(Contracts.Enums.StatusCode.Error, string.Format("Sensor does not exist for id - {0}.", sensorId));
            }

            if (sensor.Asset != null)
            {
                var detachSensor = new List<EnableSensor> { new EnableSensor { SensorKey = sensor.SensorKey } };
                await this.gatewayService.GatewayMessage(DeviceMessageStatus.DetachSensor, detachSensor);
            }

            this.repository.Delete(sensor);
            return new OperationStatus();
        }

        Sensor ISensorService.Get(int sensorId)
        {
            var sensor = this.repository.Read<Entities.Sensor>(sensorId);
            return new SensorMapping().Map(sensor);
        }

        List<Sensor> ISensorService.GetAll()
        {
            var sensor = this.repository.Query<Entities.Sensor>();
            return new SensorMapping().Map(sensor).ToList();
        }

        List<Sensor> ISensorService.GetAllUnmappedSensors()
        {
            var sensor = this.repository.Query<Entities.Sensor>().Where(s => s.SensorGroup == null);
            return new SensorMapping().Map(sensor).ToList();
        }

        List<Sensor> ISensorService.GetAllSensorBySensorType(int sensorTypeId)
        {
            var sensor = this.repository.Query<Entities.Sensor>().Where(s => s.SensorTypeId == sensorTypeId);
            return new SensorMapping().Map(sensor).ToList();
        }

        OperationStatus ISensorService.Update(Sensor sensor)
        {
            var sensorEntity = this.repository.Read<Entities.Sensor>(sensor.Id);

            if (sensorEntity == null)
            {
                return new OperationStatus(Contracts.Enums.StatusCode.Error, string.Format("Sensor does not exist for id - {0}.", sensor.Id));
            }


            var isexistingSensor = this.repository.Query<Entities.Sensor>().Any(s => s.Name.Equals(sensor.Name, StringComparison.InvariantCultureIgnoreCase) && s.Id != sensor.Id);

            if (isexistingSensor)
            {
                return new OperationStatus(StatusCode.Error, "Sensor already exists with given name.");
            }

            sensorEntity.Name = sensor.Name;
            sensorEntity.Description = sensor.Description;

            this.repository.Persist(sensorEntity);
            return new OperationStatus();
        }

        string ISensorService.GetSensorType(string sensorKey)
        {
            var sensor = this.repository.Query<Entities.Sensor>().FirstOrDefault(s => s.SensorKey.Equals(sensorKey));

            return sensor == null || sensor.SensorType == null ? null : sensor.SensorType.Name;
        }

        OperationStatus ISensorService.AddSensorToSensorType(int sensorTypeId, List<Sensor> sensors)
        {
            var sensorType = this.repository.Read<Entities.SensorType>(sensorTypeId);

            if (sensorType == null)
            {
                return new OperationStatus(Contracts.Enums.StatusCode.Error, string.Format("Sensor type does not exist for id - {0}.", sensorTypeId));
            }

            var existingSensors = this.repository.Query<Entities.Sensor>().Select(s => new Sensor { Name = s.Name, SensorKey = s.SensorKey });

            foreach (var sensor in sensors)
            {
                var isexistingSensor = existingSensors.Any(g => g.SensorKey.Equals(sensor.SensorKey, StringComparison.InvariantCultureIgnoreCase) || g.Name.Equals(sensor.Name, StringComparison.InvariantCultureIgnoreCase));

                if (!isexistingSensor)
                {
                    var sensorEntity = new Entities.Sensor
                    {
                        Name = sensor.Name,
                        SensorKey = sensor.SensorKey,
                    };

                    sensorType.Sensors.Add(sensorEntity);
                }
            }

            this.repository.Persist(sensorType);
            return new OperationStatus();
        }
    }
}
