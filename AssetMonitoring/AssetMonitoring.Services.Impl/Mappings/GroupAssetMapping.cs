namespace AssetMonitoring.Services.Impl.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Contracts;

    public class GroupAssetMapping
    {
        public IQueryable<GroupAsset> Map(IQueryable<Entities.Asset> source)
        {
            return from s in source
                   select new GroupAsset
                   {
                       AssetId = s.Id,
                       AssetBarcode = s.AssetBarcode,
                       GroupName = s.SensorGroup != null ? s.SensorGroup.Name : null,
                       SensorKeys = s.Sensors.Select(sen => sen.SensorKey).ToList()
                   };
        }

        public GroupAsset Map(Entities.Asset source)
        {
            return source == null ? null : this.Map(new List<Entities.Asset> { source }.AsQueryable()).First();
        }
    }
}
