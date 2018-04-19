namespace AssetMonitoring.Services.Impl.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Contracts;

    public class SensorMapping
    {
        public IQueryable<Sensor> Map(IQueryable<Entities.Sensor> source)
        {
            return from s in source
                   select new Sensor
                   {
                       Id = s.Id,
                       Name = s.Name,
                       Description = s.Description,
                      SensorKey = s.SensorKey,
                      SensorTypeId = s.SensorTypeId.Value,
                      SensorGroupId = s.SensorGroupId ?? default(int),
                       SensorGroupName = s.SensorGroup != null ? s.SensorGroup.Name : default(string)
                   };
        }

        public Sensor Map(Entities.Sensor source)
        {
            return source == null ? null : this.Map(new List<Entities.Sensor> { source }.AsQueryable()).First();
        }
    }
}
