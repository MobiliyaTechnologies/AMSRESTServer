namespace AssetMonitoring.Services.Impl.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Contracts;

    public class GatewayMapping 
    {
        public IQueryable<Gateway> Map(IQueryable<Entities.Gateway> source)
        {
            return from s in source
                   select new Gateway
                   {
                       Id = s.Id,
                       Name = s.Name,
                       Description = s.Description,
                       GatewayKey = s.GatewayKey
                   };
        }

        public Gateway Map(Entities.Gateway source)
        {
            return source == null ? null : this.Map(new List<Entities.Gateway> { source }.AsQueryable()).First();
        }
    }
}
