namespace AssetMonitoring.Services.Impl.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Contracts;

    public class ApplicationConfigurationMapping
    {
        public IQueryable<Configuration> Map(IQueryable<Entities.ApplicationConfiguration> source)
        {
            return from s in source
                   select new Configuration
                   {
                       ApplicationConfigurationType = s.ConfigurationType,
                       ApplicationConfigurationEntries = (from entry in s.ApplicationConfigurationEntries
                                                          select new ApplicationConfigurationEntry
                                                          {
                                                              Id = entry.Id,
                                                              ConfigurationKey = entry.ConfigurationKey,
                                                              ConfigurationValue = entry.ConfigurationValue
                                                          }).ToList()
                   };
        }

        public Configuration Map(Entities.ApplicationConfiguration source)
        {
            return source == null ? null : this.Map(new List<Entities.ApplicationConfiguration> { source }.AsQueryable()).FirstOrDefault();
        }
    }
}
