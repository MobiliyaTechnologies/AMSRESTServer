namespace AssetMonitoring.Services.Impl.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Contracts;

    public class SensorTypeMapping
    {
        public IQueryable<SensorType> Map(IQueryable<Entities.SensorType> source)
        {
            return from s in source
                   select new SensorType
                   {
                       Id = s.Id,
                       Name = s.Name,
                       Description = s.Description,
                       Capabilities = (from c in s.Capabilities
                                       select new Capability
                                       {
                                           Id = c.Id,
                                           Name = c.Name,
                                           Description = c.Description,
                                           Filters = (from f in c.SensorCapabilityFilters
                                                     select new CapabilityFilter
                                                     {
                                                         Name = f.Name,
                                                         Id = f.Id
                                                     }).ToList()
                                       }).ToList()

                   };
        }

        public SensorType Map(Entities.SensorType source)
        {
            return source == null ? null : this.Map(new List<Entities.SensorType> { source }.AsQueryable()).First();
        }
    }
}
