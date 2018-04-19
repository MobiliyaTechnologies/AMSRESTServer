namespace AssetMonitoring.Services.Impl.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Utilities;

    public class IotHubGatewayMapping
    {
        public IQueryable<IotHubGateway> Map(IQueryable<Entities.Gateway> source)
        {
            return from s in source
                   select new IotHubGateway
                   {
                      IotHubAccessTocken = s.IotHubAccessTocken,
                      IotHubHostName = ApplicationConfiguration.IotHubHostName
                   };
        }

        public IotHubGateway Map(Entities.Gateway source)
        {
            return source == null ? null : this.Map(new List<Entities.Gateway> { source }.AsQueryable()).First();
        }
    }
}
