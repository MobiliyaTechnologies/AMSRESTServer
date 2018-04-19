namespace AssetMonitoring.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Components.Repository;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Services.Impl.Mappings;

    public sealed class SensorCapabilityFilterService : ISensorCapabilityFilterService
    {
        private readonly IRepository repository;

        public SensorCapabilityFilterService(IRepository repository)
        {
            this.repository = repository;
        }

        OperationStatus ISensorCapabilityFilterService.Create(CapabilityFilter sensorCapabilityFilter)
        {
            var sensorCapability = this.repository.Read<Entities.Capability>(sensorCapabilityFilter.CapabilityId);

            if (sensorCapability == null)
            {
                return new OperationStatus(Contracts.Enums.StatusCode.Error, "Sensor capability does not exists.");
            }

            var capabilityFilter = new Entities.SensorCapabilityFilter
            {
                Name = sensorCapabilityFilter.Name,
                Description = sensorCapabilityFilter.Description,
                CapabilityId = sensorCapabilityFilter.CapabilityId
            };

            this.repository.Persist(capabilityFilter);
            return new OperationStatus();
        }

        OperationStatus ISensorCapabilityFilterService.Delete(int sensorCapabilityFilterId)
        {
            var capabilityFilter = this.repository.Read<Entities.SensorCapabilityFilter>(sensorCapabilityFilterId);

            if (capabilityFilter == null)
            {
                return new OperationStatus(Contracts.Enums.StatusCode.Error, "Sensor capability filter does not exists.");
            }

            this.repository.Delete(capabilityFilter);
            return new OperationStatus();
        }

        CapabilityFilter ISensorCapabilityFilterService.Get(int sensorCapabilityFilterId)
        {
            var capabilityFilter = this.repository.Read<Entities.SensorCapabilityFilter>(sensorCapabilityFilterId);

            return new CapabilityFilterMapping().Map(capabilityFilter);
        }

        List<CapabilityFilter> ISensorCapabilityFilterService.GetAll()
        {
            var capabilityFilters = this.repository.Query<Entities.SensorCapabilityFilter>();

            return new CapabilityFilterMapping().Map(capabilityFilters).ToList();
        }

        List<CapabilityFilter> ISensorCapabilityFilterService.GetAllFilterByCapability(int capabilityId)
        {
            var capabilityFilters = this.repository.Query<Entities.SensorCapabilityFilter>().Where(f => f.CapabilityId == capabilityId);

            return new CapabilityFilterMapping().Map(capabilityFilters).ToList();
        }

        OperationStatus ISensorCapabilityFilterService.Update(CapabilityFilter sensorCapabilityFilter)
        {
            var capabilityFilter = this.repository.Read<Entities.SensorCapabilityFilter>(sensorCapabilityFilter.Id);

            if (capabilityFilter == null)
            {
                return new OperationStatus(Contracts.Enums.StatusCode.Error, "Sensor capability filter does not exists.");
            }

            if (capabilityFilter.CapabilityId != sensorCapabilityFilter.CapabilityId)
            {
                var sensorCapability = this.repository.Read<Entities.Capability>(sensorCapabilityFilter.CapabilityId);

                if (sensorCapability == null)
                {
                    return new OperationStatus(Contracts.Enums.StatusCode.Error, "Sensor capability does not exists.");
                }
            }

            capabilityFilter.Name = sensorCapabilityFilter.Name;
            capabilityFilter.Description = sensorCapabilityFilter.Description;
            capabilityFilter.CapabilityId = sensorCapabilityFilter.CapabilityId;

            this.repository.Persist(capabilityFilter);
            return new OperationStatus();
        }
    }
}
