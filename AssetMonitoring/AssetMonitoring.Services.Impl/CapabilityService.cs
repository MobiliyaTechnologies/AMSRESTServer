namespace AssetMonitoring.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Components.Repository;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Services.Impl.Mappings;

    public class CapabilityService : ICapabilityService
    {
        private readonly IRepository repository;

        public CapabilityService(IRepository repository)
        {
            this.repository = repository;
        }

        void ICapabilityService.Create(Capability sensorCapability)
        {
            var capability = new Entities.Capability
            {
                Name = sensorCapability.Name,
                Description = sensorCapability.Description,
                Unit = sensorCapability.Unit
            };

            foreach (var filter in sensorCapability.Filters.ToList())
            {
                if (filter != null && !string.IsNullOrWhiteSpace(filter.Name))
                {
                    var sensorCapabilityFilter = new Entities.SensorCapabilityFilter
                    {
                        Name = filter.Name,
                        Description = filter.Description,
                    };

                    capability.SensorCapabilityFilters.Add(sensorCapabilityFilter);
                }
            }

            this.repository.Persist(capability);
        }

        OperationStatus ICapabilityService.Delete(int sensorCapabilityId)
        {
            var sensorCapability = this.repository.Read<Entities.Capability>(sensorCapabilityId);

            if (sensorCapability == null)
            {
                return new OperationStatus(Contracts.Enums.StatusCode.Error, "Sensor capability does not exists.");
            }

            this.repository.Delete(sensorCapability);

            return new OperationStatus(Contracts.Enums.StatusCode.Ok);
        }

        Capability ICapabilityService.Get(int sensorCapabilityId)
        {
            var sensorCapability = this.repository.Read<Entities.Capability>(sensorCapabilityId);

            return new SensorCapabilityMapping().Map(sensorCapability);
        }

        List<Capability> ICapabilityService.GetAll()
        {
            var sensorCapabilities = this.repository.Query<Entities.Capability>().ToList();

            return new SensorCapabilityMapping().Map(sensorCapabilities.AsQueryable()).ToList();
        }

        List<Capability> ICapabilityService.GetAllSensorTypeCapabilities(int sensorTypeId)
        {
            var sensorCapabilities = this.repository.Query<Entities.Capability>().Where(s => s.SensorTypes.Any(t => t.Id == sensorTypeId)).ToList();

            return new SensorCapabilityMapping().Map(sensorCapabilities.AsQueryable()).ToList();
        }

        OperationStatus ICapabilityService.Update(Capability sensorCapability)
        {
            var capability = this.repository.Read<Entities.Capability>(sensorCapability.Id);

            if (capability == null)
            {
                return new OperationStatus(Contracts.Enums.StatusCode.Error, "Sensor capability does not exists.");
            }

            var unit = sensorCapability.Unit.Trim();

            if (!capability.SupportedUnits.Split(',').Any(u => u.Equals(unit)))
            {
                return new OperationStatus(Contracts.Enums.StatusCode.Error, "Capability unit does not supported, supported units are - " + capability.SupportedUnits);
            }

            capability.Name = sensorCapability.Name;
            capability.Description = sensorCapability.Description;
            capability.Unit = unit;
            this.repository.Persist(capability);

            return new OperationStatus();
        }
    }
}
