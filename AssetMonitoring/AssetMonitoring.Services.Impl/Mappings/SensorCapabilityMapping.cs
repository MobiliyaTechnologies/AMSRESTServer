namespace AssetMonitoring.Services.Impl.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Contracts;

    public class SensorCapabilityMapping
    {
        public IQueryable<Capability> Map(IQueryable<Entities.Capability> source)
        {
            return from s in source
                   select new Capability
                   {
                       Id = s.Id,
                       Name = s.Name,
                       Description = s.Description,
                       Unit = s.Unit,
                       SupportedUnits = string.IsNullOrWhiteSpace(s.SupportedUnits) ? new List<string>() : s.SupportedUnits.Split(',').ToList(),
                       Filters = (from f in s.SensorCapabilityFilters
                                 select new CapabilityFilter
                                 {
                                     Id = f.Id,
                                     Name = f.Name,
                                     Description = f.Description,
                                     CapabilityId = f.CapabilityId.Value,
                                     MinValue = f.MinValue,
                                     MaxValue = f.MaxValue,
                                     Operator = f.Operator
                                 }).ToList()
                   };
        }

        public Capability Map(Entities.Capability source)
        {
            return source == null ? null : this.Map(new List<Entities.Capability> { source }.AsQueryable()).First();
        }
    }
}
