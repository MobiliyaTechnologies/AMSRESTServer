namespace AssetMonitoring.Services.Impl.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Contracts;

    public class SensorGroupMapping
    {
        public IQueryable<SensorGroup> Map(IQueryable<Entities.SensorGroup> source)
        {
            return from s in source
                   select new SensorGroup
                   {
                       Id = s.Id,
                       Name = s.Name,
                       Description = s.Description,
                       RuleCreationInProgress = s.RuleCreationInProgress,
                       Sensors = (from sensor in s.Sensors
                                  select new Sensor
                                  {
                                      Id = sensor.Id,
                                      Name = sensor.Name,
                                      SensorTypeId = sensor.SensorTypeId.Value,
                                      SensorKey = sensor.SensorKey
                                  }).ToList(),
                       SensorRules = (from rule in s.SensorRules
                                      select new SensorRule
                                      {
                                          CapabilityFilterId = rule.CapabilityFilterId ?? default(int),
                                          MinThreshold = rule.MinThreshold,
                                          MaxThreshold = rule.MaxThreshold,
                                          Id = rule.Id,
                                          Operator = rule.CapabilityFilter.Operator
                                      }).ToList(),
                       Assets = (from a in s.Assets
                                 select new GroupAsset
                                 {
                                     AssetId = a.Id,
                                     AssetBarcode = a.AssetBarcode,
                                     SensorKeys = a.Sensors.Select(sen => sen.SensorKey).ToList()
                                 }).ToList()
                   };
        }

        public SensorGroup Map(Entities.SensorGroup source)
        {
            return source == null ? null : this.Map(new List<Entities.SensorGroup> { source }.AsQueryable()).First();
        }
    }
}
