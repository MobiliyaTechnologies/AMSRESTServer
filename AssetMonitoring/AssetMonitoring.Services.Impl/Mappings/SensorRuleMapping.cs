
namespace AssetMonitoring.Services.Impl.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Contracts;

    public class SensorRuleMapping
    {
        public IQueryable<SensorRule> Map(IQueryable<Entities.SensorRule> source)
        {
            return from s in source
                   select new SensorRule
                   {
                       Id = s.Id,
                       MinThreshold = s.MinThreshold,
                       MaxThreshold = s.MaxThreshold,
                       SensorGroupId = s.SensorGroupId.Value,
                       CapabilityFilterId = s.CapabilityFilterId ?? default(int),
                       Capability = s.CapabilityFilter.Capability.Name,
                       SensorGroupName = s.SensorGroup.Name,
                       RuleCreationInProgress = s.SensorGroup.RuleCreationInProgress
                   };
        }

        public SensorRule Map(Entities.SensorRule source)
        {
            return source == null ? null : this.Map(new List<Entities.SensorRule> { source }.AsQueryable()).First();
        }
    }
}
