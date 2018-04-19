namespace AssetMonitoring.Services.Impl.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Contracts;

    public class EnableSensorMapping
    {
        public IQueryable<EnableSensor> Map(IQueryable<Entities.Sensor> source)
        {
            return from s in source
                   select new EnableSensor
                   {
                       SensorKey = s.SensorKey,
                       SensorType = s.SensorType.Name,
                       GroupId = s.SensorGroupId ?? default(int),
                       AssetBarcode = s.Asset.AssetBarcode                      
                   };
        }

        public EnableSensor Map(Entities.Sensor source)
        {
            return source == null ? null : this.Map(new List<Entities.Sensor> { source }.AsQueryable()).First();
        }
    }
}
