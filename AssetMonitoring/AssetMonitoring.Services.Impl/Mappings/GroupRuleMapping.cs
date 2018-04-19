
namespace AssetMonitoring.Services.Impl.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts.AnalyticsContract;

    public class GroupRuleMapping
    {
        public IQueryable<GroupRule> Map(IQueryable<Entities.SensorRule> source)
        {
            return from s in source
                   select new GroupRule
                   {
                       RuleId = s.Id,
                       CapabilityId = s.CapabilityFilter.CapabilityId ?? default(int),
                       Filter = s.CapabilityFilter.Name,
                       Operator = s.CapabilityFilter.Operator,
                       MinThreshold = s.MinThreshold,
                       MaxThreshold = s.MaxThreshold
                   };
        }

        public GroupRule Map(Entities.SensorRule source)
        {
            return source == null ? null : this.Map(new List<Entities.SensorRule> { source }.AsQueryable()).First();
        }
    }
}
