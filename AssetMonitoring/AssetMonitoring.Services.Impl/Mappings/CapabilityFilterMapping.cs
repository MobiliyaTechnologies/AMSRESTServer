namespace AssetMonitoring.Services.Impl.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Contracts;

    public class CapabilityFilterMapping
    {
        public IQueryable<CapabilityFilter> Map(IQueryable<Entities.SensorCapabilityFilter> source)
        {
            return from s in source
                   select new CapabilityFilter
                   {
                       Id = s.Id,
                       Name = s.Name,
                       Description = s.Description,
                       CapabilityId = s.CapabilityId.Value,
                       MinValue = s.MinValue,
                       MaxValue = s.MaxValue,
                       Operator = s.Operator
                   };
        }

        public CapabilityFilter Map(Entities.SensorCapabilityFilter source)
        {
            return source == null ? null : this.Map(new List<Entities.SensorCapabilityFilter> { source }.AsQueryable()).First();
        }
    }
}
