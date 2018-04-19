namespace AssetMonitoring.Services.Impl.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Contracts;

    public class IndoorLayoutMapping
    {
        public IQueryable<IndoorLayout> Map(IQueryable<Entities.IndoorLayout> source)
        {
            return from s in source
                   select new IndoorLayout
                   {
                       Id = s.Id,
                       Name = s.Name,
                       Description = s.Description,
                       FileName = s.FileName,
                       Gateways = (from g in s.Gateways
                                   select new Gateway
                                   {
                                       Id = g.Id,
                                       Name = g.Name,
                                       GatewayKey = g.GatewayKey,
                                       LayoutX = g.LayoutX,
                                       LayoutY = g.LayoutY
                                   }).ToList()
                   };
        }

        public IndoorLayout Map(Entities.IndoorLayout source)
        {
            return source == null ? null : this.Map(new List<Entities.IndoorLayout> { source }.AsQueryable()).First();
        }
    }
}
